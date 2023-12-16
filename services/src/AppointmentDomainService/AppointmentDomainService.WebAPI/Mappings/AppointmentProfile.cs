using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Commands;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.MobileClient;
using AppointmentListRequestDto = Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto.AppointmentListRequestDto;
using AppointmentListResponseDto = Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class AppointmentsProfile : Profile
    {
        public AppointmentsProfile()
        {
            CreateMap<Appointment, NearestAppointmentInfoResponseDto>()
                .ForMember(dest => dest.AppointmentId, opt =>
                    opt.MapFrom(src => src.Id))
                .DisableCtorValidation();

            CreateMap<Appointment, API.Common.MobileClientBffQueryService.Dto.AppointmentResponseDto>()
                .ForMember(dest => dest.Attendees, opt =>
                    opt.MapFrom(src => src.Attendees.Select(a => a.UserId).ToArray()));
            CreateMap<List<Appointment>, AppointmentListResponseDto>()
                .ForMember(dest => dest.Appointments, opt =>
                    opt.MapFrom(src => src));

            CreateMap<Appointment, API.Common.WebClientBFFQueryService.Dto.AppointmentResponseDto>()
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees.Select(x => x.UserId).ToList()))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.GetState()));

            CreateMap<IEnumerable<Appointment>, API.Common.WebClientBFFQueryService.Dto.AppointmentListResponseDto>()
                .ForMember(dest => dest.Items, opt =>
                    opt.MapFrom(src => new List<Appointment>(src)))
                .ForMember(dest => dest.Paging, opt => opt.Ignore())
                .DisableCtorValidation();

            CreateMap<AppointmentListRequestDto, GetAppointmentListQuery>();
            CreateMap<ChangedAppointmentListRequestDto, GetChangedAppointmentListQuery>();
            CreateMap<API.Common.WebClientBFFQueryService.Dto.AppointmentListRequestDto, Queries.WebClient.GetAppointmentListQuery>();

            CreateMap<UpdateAppointmentStatusRequestDto, UpdateAppointmentStatusCommand>();
            CreateMap<AppointmentState, API.Common.Common.AppointmentState>().ReverseMap();

            CreateMap<CreateAppointmentRequestDto, CreateAppointmentCommand>(MemberList.Source)
                .ForCtorParam("type", opt => opt.MapFrom(src => src.AppointmentType));
            CreateMap<CreateAppointmentCommand, CreateAppointmentDomainDto>();
            CreateMap<RescheduleAppointmentRequestDto, RescheduleAppointmentCommand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<RescheduleAppointmentCommand, RescheduleAppointmentDomainDto>();

            CreateMap<API.Common.Common.AppointmentType, AppointmentType>().ReverseMap();

            CreateMap<Appointment, AppointmentByIdResponseDto>()
                .ForMember(dest => dest.Attendees, opt =>
                    opt.MapFrom(src => src.Attendees.Select(a => a.UserId)));
        }
    }
}
