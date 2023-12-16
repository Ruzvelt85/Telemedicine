using System.Collections.Generic;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Mappings
{
    public class BloodPressureMeasurementProfile : Profile
    {
        public BloodPressureMeasurementProfile()
        {
            CreateMap<CreateMeasurementRequestDto<BloodPressureMeasurementDto>, CreateMeasurementCommand<BloodPressureMeasurementDto>>();
            CreateMap<CreateMeasurementCommand<BloodPressureMeasurementDto>, BloodPressureMeasurement>(MemberList.Source)
                .ForCtorParam("systolic", opt => opt.MapFrom(src => src.Measure.Systolic))
                .ForCtorParam("diastolic", opt => opt.MapFrom(src => src.Measure.Diastolic))
                .ForCtorParam("pulseRate", opt => opt.MapFrom(src => src.Measure.PulseRate))
                .ForSourceMember(src => src.Measure, opt => opt.DoNotValidate());

            CreateMap<IEnumerable<BloodPressureMeasurement>, PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Paging, opt => opt.Ignore());
            CreateMap<BloodPressureMeasurement, MeasurementResponseDto<BloodPressureMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src));
            CreateMap<BloodPressureMeasurement, BloodPressureMeasurementDto>(MemberList.Destination);
        }
    }
}
