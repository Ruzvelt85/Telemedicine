using AutoMapper;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using DomainService = Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Mappers
{
    public class PulseRateMeasurementProfile : Profile
    {
        public PulseRateMeasurementProfile()
        {
            CreateMap<CreatePulseRateMeasurementRequestDto, CreatePulseRateMeasurementCommand>();

            CreateMap<CreatePulseRateMeasurementCommand, CreateMeasurementRequestDto<DomainService.PulseRateMeasurementDto>>()
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<CreatePulseRateMeasurementCommand, DomainService.PulseRateMeasurementDto>();

            CreateMap<CreateSaturationMeasurementCommand, CreateMeasurementRequestDto<DomainService.PulseRateMeasurementDto>>()
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<CreateSaturationMeasurementCommand, DomainService.PulseRateMeasurementDto>();

            CreateMap<CreateBloodPressureMeasurementCommand, CreateMeasurementRequestDto<DomainService.PulseRateMeasurementDto>>()
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<CreateBloodPressureMeasurementCommand, DomainService.PulseRateMeasurementDto>();
        }
    }
}
