using AutoMapper;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Mappings
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<DoctorListRequestDto, GetDoctorListQuery>();
            CreateMap<Doctor, DoctorResponseDto>();

            CreateMap<DoctorByInnerIdRequestDto, GetDoctorByInnerIdQuery>();
            CreateMap<Doctor, DoctorByInnerIdResponseDto>();

            CreateMap<CreateOrUpdateDoctorRequestDto, CreateOrUpdateDoctorCommand>();

            CreateMap<CreateOrUpdateDoctorCommand, CreateDoctorCommand>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsActive.HasValue && !src.IsActive.Value))
                .ConstructUsing(x => new CreateDoctorCommand(x.InnerId, x.LastName, x.FirstName));

            CreateMap<CreateOrUpdateDoctorCommand, UpdateDoctorCommand>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsActive.HasValue && !src.IsActive.Value))
                .ConstructUsing(x => new UpdateDoctorCommand(x.LastName, x.FirstName));

            CreateMap<CreateDoctorCommand, Doctor>(MemberList.Source);
            CreateMap<UpdateDoctorCommand, Doctor>(MemberList.Source);
        }
    }
}
