using System.Collections.Generic;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Mappings
{
    public class PulseRateMeasurementProfile : Profile
    {
        public PulseRateMeasurementProfile()
        {
            CreateMap<CreateMeasurementRequestDto<PulseRateMeasurementDto>, CreateMeasurementCommand<PulseRateMeasurementDto>>();
            CreateMap<CreateMeasurementCommand<PulseRateMeasurementDto>, PulseRateMeasurement>(MemberList.Source)
                .ForCtorParam("pulseRate", opt => opt.MapFrom(src => src.Measure.PulseRate))
                .ForSourceMember(src => src.Measure, opt => opt.DoNotValidate());

            CreateMap<IEnumerable<PulseRateMeasurement>, PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Paging, opt => opt.Ignore());
            CreateMap<PulseRateMeasurement, MeasurementResponseDto<PulseRateMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src));
            CreateMap<PulseRateMeasurement, PulseRateMeasurementDto>(MemberList.Destination);
        }
    }
}
