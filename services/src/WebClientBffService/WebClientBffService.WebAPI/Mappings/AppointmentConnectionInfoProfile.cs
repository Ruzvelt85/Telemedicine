using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Mappings
{
    public class AppointmentConnectionInfoProfile : Profile
    {
        public AppointmentConnectionInfoProfile()
        {
            CreateMap<AppointmentByIdResponseDto, AppointmentInfoDto>();

            CreateMap<ConnectionInfoResponseDto, AppointmentConnectionInfoResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AppointmentId));
        }
    }
}
