using AutoMapper;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using DomainService = Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Mappers
{
    public class BloodPressureMeasurementProfile : Profile
    {
        public BloodPressureMeasurementProfile()
        {
            CreateMap<CreateBloodPressureMeasurementRequestDto, CreateBloodPressureMeasurementCommand>();

            CreateMap<CreateBloodPressureMeasurementCommand, CreateMeasurementRequestDto<DomainService.BloodPressureMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<CreateBloodPressureMeasurementCommand, DomainService.BloodPressureMeasurementDto>(MemberList.Destination);
        }
    }
}
