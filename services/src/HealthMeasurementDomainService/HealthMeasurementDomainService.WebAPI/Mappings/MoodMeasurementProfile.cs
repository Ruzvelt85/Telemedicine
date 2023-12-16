using System.Collections.Generic;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Mappings
{
    public class MoodMeasurementProfile : Profile
    {
        public MoodMeasurementProfile()
        {
            CreateMap<CreateMeasurementRequestDto<MoodMeasurementDto>, CreateMeasurementCommand<MoodMeasurementDto>>();
            CreateMap<CreateMeasurementCommand<MoodMeasurementDto>, MoodMeasurement>(MemberList.None)
                .ForCtorParam("measure", opt => opt.MapFrom(src => src.Measure.Measure))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.ClientDate, opt => opt.MapFrom(src => src.ClientDate));

            CreateMap<IEnumerable<MoodMeasurement>, PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Paging, opt => opt.Ignore());
            CreateMap<MoodMeasurement, MeasurementResponseDto<MoodMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src));
            CreateMap<MoodMeasurement, MoodMeasurementDto>(MemberList.Destination);
        }
    }
}
