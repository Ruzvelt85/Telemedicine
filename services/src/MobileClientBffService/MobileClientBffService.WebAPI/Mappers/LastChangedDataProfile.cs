using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Telemedicine.Services.MobileClientBffService.API;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Queries;
using AppointmentCommon = Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using AppointmentResponseDto = Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto.AppointmentResponseDto;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Mappers
{
    public class LastChangedDataProfile : Profile
    {
        public LastChangedDataProfile()
        {
            CreateMap<LastChangedDataRequestDto, GetLastChangedDataQuery>();

            CreateMap<HealthCenterStructureDomainService.API.Common.Common.UserType, UserType>();

            CreateMap<HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto.UserInfoResponseDto, AttendeeResponseDto>()
                .ForMember(dest => dest.UserType, opt =>
                    opt.MapFrom(src => src.Type));

            CreateMap<IDictionary<Guid, AttendeeResponseDto[]>, IReadOnlyCollection<AppointmentResponseDto>>()
                .ConvertUsing((src, dest) => dest.Select(el => el with { Attendees = src[el.Id] }).ToArray());

            // Automapper cannot find in which constructor parameter to map the state, we indicate explicitly 
            CreateMap<AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto.AppointmentResponseDto, AppointmentResponseDto>(MemberList.Source)
                .ForMember(dest => dest.Attendees, opt => opt.Ignore())
                .ForCtorParam("status", opt => opt.MapFrom(_ => _.State));

            CreateMap<AppointmentCommon.AppointmentState, AppointmentStatus>()
                .ConvertUsing(x => StatusFromState(x));


            CreateMap<HealthMeasurementDomain.Mood.Dto.MoodMeasureType, MoodMeasureType>();

            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Mood.Dto.MoodMeasurementDto>, MoodResponseDto>(MemberList.Source)
                .IncludeMembers(src => src.Measure)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src.Measure.Measure));

            CreateMap<HealthMeasurementDomain.Mood.Dto.MoodMeasurementDto, MoodResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientDate, opt => opt.Ignore());
        }

        private static AppointmentStatus StatusFromState(AppointmentCommon.AppointmentState state) =>
            (state) switch
            {
                AppointmentCommon.AppointmentState.Default => AppointmentStatus.Default,
                AppointmentCommon.AppointmentState.Opened => AppointmentStatus.Opened,
                AppointmentCommon.AppointmentState.Ongoing => AppointmentStatus.Opened,
                AppointmentCommon.AppointmentState.Cancelled => AppointmentStatus.Cancelled,
                AppointmentCommon.AppointmentState.Missed => AppointmentStatus.Cancelled,
                AppointmentCommon.AppointmentState.Done => AppointmentStatus.Done,
                _ => AppointmentStatus.Default
            };
    }
}
