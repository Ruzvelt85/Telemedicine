using AutoMapper;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientById;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients;
using AppointmentServiceDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using HealthCenterStructureServiceDto = Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Mappings
{
    public class PatientListProfile : Profile
    {
        public PatientListProfile()
        {
            CreateMap<PatientListRequestDto, GetPatientListQuery>();

            CreateMap<PatientListFilterRequestDto, HealthCenterStructureServiceDto.GetPatientList.PatientListFilterRequestDto>(MemberList.Source);

            CreateMap<HealthCenterStructureServiceDto.GetPatientList.HealthCenterStructureFilterType, HealthCenterStructureFilterType>();

            CreateMap<GetPatientListQuery, HealthCenterStructureServiceDto.GetPatientList.PatientListRequestDto>();

            CreateMap<AppointmentServiceDto.NearestAppointmentInfoResponseDto, PatientListNearestAppointmentInfoResponseDto>();

            CreateMap<HealthCenterStructureServiceDto.GetPatientList.PatientResponseDto, PatientResponseDto>(MemberList.Source)
                .ForSourceMember(src => src.InnerId, dest => dest.DoNotValidate());

            CreateMap<HealthCenterStructureServiceDto.GetPatientById.PatientByIdResponseDto, PatientByIdResponseDto>();
            CreateMap<HealthCenterStructureDomainService.API.Common.Common.HealthCenterResponseDto, HealthCenterResponseDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.UsaState));

            CreateMap<HealthCenterStructureServiceDto.GetPatientById.PrimaryCareProviderResponseDto, PrimaryCareProviderResponseDto>();

            CreateMap<AppointmentServiceDto.NearestAppointmentsResponseDto,
                        PatientResponseDto>()
                    .ForMember(dest => dest.PreviousAppointmentInfo,
                        orig => orig.MapFrom(_ => _.PreviousAppointmentInfo))
                    .ForMember(dest => dest.NextAppointmentInfo,
                        orig => orig.MapFrom(_ => _.NextAppointmentInfo))
                    .ForMember(dest => dest.NextAppointmentType,
                        orig => orig.MapFrom(_ => _.NextAppointmentType))
                    .ForAllOtherMembers(x => x.Ignore());
        }
    }
}
