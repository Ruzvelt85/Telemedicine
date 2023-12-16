using System;
using AutoMapper;
// ReSharper disable UnusedParameter.Local

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender.Mappings
{
    /// <summary>
    /// This class contains a Mapper configured for this project
    /// inspired by https://stackoverflow.com/a/49456783/5367706
    /// </summary>
    internal static class SqsMapping
    {
        private static readonly Lazy<IMapper> _lazyMapper = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod!.IsPublic || p.GetMethod.IsAssembly; // This line ensures that internal properties are also mapped over.
                cfg.AddProfile<AwsSqsConfigsMappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        /// <summary>
        /// The mapper configured for this project
        /// </summary>
        public static IMapper Mapper => _lazyMapper.Value;
    }
}
