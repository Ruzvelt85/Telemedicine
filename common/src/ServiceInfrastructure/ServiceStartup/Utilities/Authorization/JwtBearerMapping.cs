using System;
using AutoMapper;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    internal static class JwtBearerMapping
    {
        private static readonly Lazy<IMapper> _lazyMapper = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod!.IsPublic || p.GetMethod.IsAssembly; // This line ensures that internal properties are also mapped over.
                cfg.AddProfile<JwtBearerSettingsMappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        /// <summary>
        /// The mapper configured for <see cref="JwtBearerSettings"/>
        /// </summary>
        public static IMapper Mapper => _lazyMapper.Value;
    }
}
