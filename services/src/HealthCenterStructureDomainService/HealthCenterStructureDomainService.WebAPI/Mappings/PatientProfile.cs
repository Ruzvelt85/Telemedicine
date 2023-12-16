using System;
using AutoMapper;
using System.Collections.Generic;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientById;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientResponseDto>();

            CreateMap<PatientListRequestDto, GetPatientsQuery>();

            CreateMap<IEnumerable<Patient>, PatientListResponseDto>()
                .ForMember(dest => dest.Items, opt =>
                    opt.MapFrom(src => new List<Patient>(src)))
                .ForMember(dest => dest.Paging, opt => opt.Ignore())
                .DisableCtorValidation();

            CreateMap<Patient, PatientByIdResponseDto>();
            CreateMap<Doctor, PrimaryCareProviderResponseDto>();

            CreateMap<CreateOrUpdatePatientRequestDto, CreateOrUpdatePatientCommand>();

            CreateMap<CreateOrUpdatePatientCommand, CreatePatientCommand>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsActive.HasValue && !src.IsActive.Value))
                .ForMember(dest => dest.HealthCenterId, opt => opt.Ignore())
                .ForMember(dest => dest.PrimaryCareProviderId, opt => opt.Ignore())
                .ConstructUsing(x => new CreatePatientCommand(x.InnerId, x.LastName, x.FirstName, x.PhoneNumber, x.BirthDate, Guid.Empty, null));

            CreateMap<CreateOrUpdatePatientCommand, UpdatePatientCommand>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsActive.HasValue && !src.IsActive.Value))
                .ForMember(dest => dest.HealthCenterId, opt => opt.Ignore()) //we are ignoring these properties because they don't exist on destination. It would be better just to disable validation on the properties
                .ForMember(dest => dest.PrimaryCareProviderId, opt => opt.Ignore())
                .ConstructUsing(x => new UpdatePatientCommand(x.LastName, x.FirstName, x.PhoneNumber, x.BirthDate, Guid.Empty, null));

            CreateMap<CreatePatientCommand, Patient>(MemberList.Source);
            CreateMap<UpdatePatientCommand, Patient>(MemberList.Source)
                .ForMember(dest => dest.InnerId, opt => opt.UseDestinationValue());
        }
    }
}
