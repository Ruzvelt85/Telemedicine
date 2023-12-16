using AutoMapper;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using DomainService = Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Mappers
{
    public class SaturationMeasurementProfile : Profile
    {
        public SaturationMeasurementProfile()
        {
            AllowNullCollections = true;

            CreateMap<CreateSaturationMeasurementRequestDto, CreateSaturationMeasurementCommand>();
            CreateMap<RawSaturationItemRequestDto, RawSaturationItemCommandDto>();

            CreateMap<CreateSaturationMeasurementCommand, CreateMeasurementRequestDto<DomainService.SaturationMeasurementDto>>(MemberList.Destination)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());

            CreateMap<CreateSaturationMeasurementCommand, DomainService.SaturationMeasurementDto>(MemberList.Destination);
            CreateMap<RawSaturationItemCommandDto, DomainService.RawSaturationMeasurementItemDto>();
        }
    }
}
