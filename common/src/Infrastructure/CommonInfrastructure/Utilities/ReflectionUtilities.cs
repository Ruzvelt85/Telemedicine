using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities
{
    /// <summary>
    /// Reflection  utility methods
    /// </summary>
    [PublicAPI]
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Returns Current executing assembly. Initially invoke GetEntryAssembly, then GetCallingAssembly
        /// </summary>
        public static Assembly ExecutingAssembly => Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        /// <summary>
        /// Returns version of <see cref="ExecutingAssembly"/>
        /// </summary>
        public static string? GetExecutingAssemblyVersion() =>
            ExecutingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        /// <summary>
        /// Returns name <see cref="Assembly.FullName"/> of <see cref="ExecutingAssembly"/>
        /// </summary>
        public static string? GetExecutingAssemblyName() => ExecutingAssembly.FullName;

        /// <summary>
        /// Return all assemblies which name (<see cref="Assembly.FullName"/>) starts with 'Telemedicine.'
        /// </summary>
        public static IEnumerable<Assembly> GetAllTelemedicineAssemblies() =>
            Directory.GetFiles(Directory.GetParent(typeof(ReflectionUtilities).Assembly.Location)!.FullName, "Telemedicine.*.dll")
                .Select(Assembly.LoadFrom);

        /// <summary>
        /// Return all referenced assemblies which name (<see cref="AssemblyName.Name"/>) starts with 'Telemedicine.'
        /// </summary>
        public static IEnumerable<Assembly> GetReferencedTelemedicineAssemblies(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies()
                .Where(a => a.Name != null && a.Name.StartsWith("Telemedicine"))
                .Select(a => Assembly.Load(a.FullName));
        }
    }
}
