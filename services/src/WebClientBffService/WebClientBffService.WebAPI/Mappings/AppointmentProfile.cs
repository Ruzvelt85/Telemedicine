using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentById;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments;
using GetAppointmentListAttendeeResponseDto = Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList.AttendeeResponseDto;
using DomainDto = Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using DomainCommon = Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<AppointmentListRequestDto, GetAppointmentListQuery>();
            CreateMap<GetAppointmentListQuery, AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentListRequestDto>();

            CreateMap<AppointmentListFilterRequestDto, AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentListFilterRequestDto>(MemberList.Source)
                .ForMember(dest => dest.AttendeeId, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.AppointmentStates, src => src.MapFrom(x => x.AppointmentStatus))
                .ForSourceMember(_ => _.AppointmentStatus, opt => opt.DoNotValidate());

            CreateMap<DomainCommon.AppointmentType, AppointmentType>();

            CreateMap<AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentResponseDto, AppointmentResponseDto>(MemberList.Source)
                .ForMember(dest => dest.Attendees, orig => orig.Ignore())
                .ForMember(dest => dest.Status, src => src.MapFrom(x => x.State));

            CreateMap<UserInfoDetailsResponseDto, GetAppointmentListAttendeeResponseDto>()
                .Include<PatientInfoResponseDto, GetAppointmentListAttendeeResponseDto>()
                .Include<DoctorInfoResponseDto, GetAppointmentListAttendeeResponseDto>()
                .ForMember(dest => dest.BirthDate, opt
                    => opt.Ignore())
                .ForMember(dest => dest.UserType, opt =>
                    opt.MapFrom(src => src.Type));

            CreateMap<PatientInfoResponseDto, GetAppointmentListAttendeeResponseDto>()
                .ForMember(dest => dest.BirthDate, opt =>
                    opt.MapFrom(src => src.BirthDate));

            CreateMap<DoctorInfoResponseDto, GetAppointmentListAttendeeResponseDto>();

            CreateMap<IDictionary<Guid, GetAppointmentListAttendeeResponseDto[]>, IReadOnlyCollection<AppointmentResponseDto>>()
                .ConvertUsing((src, dest) => dest.Select(el => el with { Attendees = src[el.Id] }).ToArray());

            CreateMap<HealthCenterStructureDomainService.API.Common.Common.UserType, UserType>();

            CreateMap<CreateAppointmentRequestDto, CreateAppointmentCommand>();
            CreateMap<CancelAppointmentRequestDto, CancelAppointmentCommand>();
            CreateMap<RescheduleAppointmentRequestDto, RescheduleAppointmentCommand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CancelAppointmentCommand, AppointmentDomainService.API.Common.AppointmentCommandService.Dto.UpdateAppointmentStatusRequestDto>()
                .ForMember(dest => dest.Status, opt =>
                    opt.MapFrom(src => DomainDto.AppointmentStatus.Cancelled));

            CreateMap<CreateAppointmentCommand, DomainDto.CreateAppointmentRequestDto>()
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore());

            CreateMap<RescheduleAppointmentCommand, DomainDto.RescheduleAppointmentRequestDto>()
                .ForMember(dest => dest.CreatorId, opt => opt.Ignore());

            #region ApointmentById

            // Automapper cannot find in which constructor parameter to map the state, we indicate explicitly 
            CreateMap<AppointmentDomainService.API.Common.AppointmentQueryService.Dto.AppointmentByIdResponseDto, AppointmentByIdResponseDto>(MemberList.Source)
                .ForMember(dest => dest.Attendees, opt => opt.Ignore())
                .ForCtorParam("status", opt => opt.MapFrom(_ => _.State));

            CreateMap<UserInfoDetailsResponseDto, API.Appointments.AppointmentQueryService.Dto.GetAppointmentById.AttendeeResponseDto>()
                .Include<PatientInfoResponseDto, API.Appointments.AppointmentQueryService.Dto.GetAppointmentById.AttendeeResponseDto>()
                .Include<DoctorInfoResponseDto, API.Appointments.AppointmentQueryService.Dto.GetAppointmentById.AttendeeResponseDto>()
                .ForMember(dest => dest.UserType, opt =>
                    opt.MapFrom(src => src.Type));
            CreateMap<PatientInfoResponseDto, API.Appointments.AppointmentQueryService.Dto.GetAppointmentById.AttendeeResponseDto>();
            CreateMap<DoctorInfoResponseDto, API.Appointments.AppointmentQueryService.Dto.GetAppointmentById.AttendeeResponseDto>();

            #endregion

            CreateMap<DomainCommon.AppointmentState, AppointmentStatus>()
                .ConvertUsing(x => StatusFromState(x));

            CreateMap<AppointmentStatus, IReadOnlyCollection<DomainCommon.AppointmentState>>()
                .ConvertUsing(x => StateFromStatus(x));
        }

        private static AppointmentStatus StatusFromState(DomainCommon.AppointmentState state) =>
            state switch
            {
                DomainCommon.AppointmentState.Opened => AppointmentStatus.Opened,
                DomainCommon.AppointmentState.Ongoing => AppointmentStatus.Opened,
                DomainCommon.AppointmentState.Cancelled => AppointmentStatus.Cancelled,
                DomainCommon.AppointmentState.Missed => AppointmentStatus.Cancelled,
                DomainCommon.AppointmentState.Done => AppointmentStatus.Done,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, $"Cannot transform state {state} into any status. Invalid operation.")
            };

        private static IReadOnlyCollection<DomainCommon.AppointmentState> StateFromStatus(AppointmentStatus status) =>
            status switch
            {
                AppointmentStatus.All => new[] { DomainCommon.AppointmentState.All },
                AppointmentStatus.Opened => new[] { DomainCommon.AppointmentState.Opened, DomainCommon.AppointmentState.Ongoing },
                AppointmentStatus.Cancelled => new[] { DomainCommon.AppointmentState.Cancelled, DomainCommon.AppointmentState.Missed },
                AppointmentStatus.Done => new[] { DomainCommon.AppointmentState.Done },
                _ => new[] { DomainCommon.AppointmentState.Default },
            };
    }
}
