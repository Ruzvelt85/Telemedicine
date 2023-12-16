using AutoMapper;
using System.Collections.Generic;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Mappings
{
    public class SaturationMeasurementProfile : Profile
    {
        public SaturationMeasurementProfile()
        {
            AllowNullCollections = true;

            CreateMap<CreateMeasurementRequestDto<SaturationMeasurementDto>, CreateMeasurementCommand<SaturationMeasurementDto>>();
            CreateMap<CreateMeasurementCommand<SaturationMeasurementDto>, SaturationMeasurement>(MemberList.Source)
                .ForCtorParam("pulseRate", opt => opt.MapFrom(src => src.Measure.PulseRate))
                .ForCtorParam("spO2", opt => opt.MapFrom(src => src.Measure.SpO2))
                .ForCtorParam("pi", opt => opt.MapFrom(src => src.Measure.Pi))
                .ForCtorParam("rawSaturationData", opt => opt.MapFrom(src => src.Measure.RawMeasurements))
                .ForSourceMember(src => src.Measure, opt => opt.DoNotValidate());

            CreateMap<IReadOnlyCollection<RawSaturationMeasurementItemDto>, RawSaturationData?>(MemberList.None)
                .ConstructUsing((src, ctx) =>
                    src is null ? null : new RawSaturationData(ctx.Mapper.Map<ICollection<RawSaturationItem>>(src)));
            CreateMap<RawSaturationMeasurementItemDto, RawSaturationItem>(MemberList.Source);

            CreateMap<IEnumerable<SaturationMeasurement>, PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Paging, opt => opt.Ignore());
            CreateMap<SaturationMeasurement, MeasurementResponseDto<SaturationMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src));
            CreateMap<SaturationMeasurement, SaturationMeasurementDto>(MemberList.Destination)
                .ForMember(dest => dest.RawMeasurements, opt => opt.MapFrom(src => src.RawSaturationData));

            CreateMap<RawSaturationData, IReadOnlyCollection<RawSaturationMeasurementItemDto>>(MemberList.None)
                .ConstructUsing(
#pragma warning disable CS8603 // Possible null reference return.
                (src, context) => src is null ? null : context.Mapper.Map<IReadOnlyCollection<RawSaturationMeasurementItemDto>>(src.Items));
#pragma warning restore CS8603 // Possible null reference return.
            CreateMap<RawSaturationItem, RawSaturationMeasurementItemDto>();
        }
    }
}
