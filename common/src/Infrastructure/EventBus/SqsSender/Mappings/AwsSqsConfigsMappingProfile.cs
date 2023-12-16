using System;
using Amazon;
using Amazon.SQS;
using AutoMapper;
// ReSharper disable UnusedParameter.Local

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender.Mappings
{
    public class AwsSqsConfigsMappingProfile : Profile
    {
        public AwsSqsConfigsMappingProfile()
        {
            /* we need these mappings to preserve source value if the destination value == null, otherwise it will be set to default: 0, false */
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<long?, long>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<TimeSpan?, TimeSpan>().ConvertUsing((src, dest) => src ?? dest);

            CreateMap<SqsSettings.AmazonSqsClientSettings, AmazonSQSConfig>()
                .ForMember(dest => dest.RegionEndpoint, opt =>
                    opt.MapFrom(src => RegionEndpoint.GetBySystemName(src.RegionEndpoint)))
                .ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null)); // do not map if the source value is null
        }
    }
}
