using AutoMapper;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using DomainService = Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Mappers
{
    public class MoodMeasurementProfile : Profile
    {
        public MoodMeasurementProfile()
        {
            CreateMap<CreateMoodMeasurementRequestDto, CreateMoodMeasurementCommand>();

            CreateMap<CreateMoodMeasurementCommand, CreateMeasurementRequestDto<DomainService.MoodMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<CreateMoodMeasurementCommand, DomainService.MoodMeasurementDto>(MemberList.Destination);
        }
    }
}
