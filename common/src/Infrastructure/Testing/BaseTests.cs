using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Xunit.Sdk;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;

namespace Telemedicine.Common.Infrastructure.Testing
{
    public class BaseTests
    {
        private readonly Assembly _assembly;

        public BaseTests(Assembly assembly)
        {
            _assembly = assembly;
        }

        public void RunTests()
        {
            RunAutomapperTest();
            RunExceptionTests();
        }

        public void RunAutomapperTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(_assembly);
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true || p.GetMethod?.IsPrivate == true;
            });

            var mapper = config.CreateMapper();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        /// <summary>
        /// Check if all ServiceLayerException derived types have
        /// ctor for restore exception right in other projects
        /// </summary>
        public void RunExceptionTests()
        {
            var referencedAssemblies = ReflectionUtilities.GetReferencedTelemedicineAssemblies(_assembly);

            var exceptionsWithoutRestoreCtor = new List<Type>();
            var exceptionsWithoutObsoleteAttributeOnCtor = new List<Type>();

            foreach (var assembly in referencedAssemblies)
            {
                var exceptionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ServiceLayerException)));
                foreach (var type in exceptionTypes)
                {
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    var ctor = type.GetConstructor(flags, null, new[] { typeof(string), typeof(IDictionary), typeof(Exception) }, null);
                    if (ctor == null)
                    {
                        exceptionsWithoutRestoreCtor.Add(type);
                        continue;
                    }

                    if (ctor.GetCustomAttribute(typeof(ObsoleteAttribute)) == null)
                    {
                        exceptionsWithoutObsoleteAttributeOnCtor.Add(type);
                    }
                }
            }

            if (exceptionsWithoutRestoreCtor.Count != 0)
            {
                throw new XunitException("Following exception types don't contain restore constructor:\n" +
                                         $"{String.Join('\n', exceptionsWithoutRestoreCtor)}");
            }

            if (exceptionsWithoutObsoleteAttributeOnCtor.Count != 0)
            {
                throw new XunitException("Following types don't contain restore constructor with obsolete attribute:\n" +
                                         $"{String.Join('\n', exceptionsWithoutObsoleteAttributeOnCtor)}");
            }
        }
    }
}
