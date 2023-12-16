using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Mappings
{
    public class AppointmentListByPatientIdProfile : Profile
    {
        public AppointmentListByPatientIdProfile()
        {
            CreateMap<AppointmentListByPatientIdRequestDto, GetAppointmentListByPatientIdQuery>(MemberList.Source);
            CreateMap<GetAppointmentListByPatientIdQuery, AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentListRequestDto>()
                .ForMember(dest => dest.Filter, opt => opt.MapFrom(src => src.Filter))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Paging))
                .ForPath(dest => dest.Filter.AttendeeId, opt => opt.MapFrom(src => src.PatientId));

            CreateMap<AppointmentListByPatientIdFilterRequestDto, AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentListFilterRequestDto>(MemberList.Source)
                .ForMember(dest => dest.AttendeeId, opt =>
                    opt.Ignore())
                .ForMember(dest => dest.AppointmentStates, src => src.MapFrom(x => x.AppointmentStatus))
                .ForSourceMember(_ => _.AppointmentStatus, opt => opt.DoNotValidate());

            CreateMap<AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentResponseDto, AppointmentResponseDto>(MemberList.Source)
                .ForMember(dest => dest.Attendees, orig => orig.Ignore())
                .ForMember(dest => dest.Status, src => src.MapFrom(x => x.State));

            CreateMap<UserInfoResponseDto, AttendeeResponseDto>()
                .ForMember(dest => dest.UserType, opt =>
                    opt.MapFrom(src => src.Type));

            CreateMap<IDictionary<Guid, AttendeeResponseDto[]>, IReadOnlyCollection<AppointmentResponseDto>>()
                .ConvertUsing((src, dest) => dest.Select(el => el with { Attendees = src[el.Id] }).ToArray());
        }
    }
}
