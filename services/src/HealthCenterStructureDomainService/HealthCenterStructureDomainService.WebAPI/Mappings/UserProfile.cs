using AutoMapper;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserInfoResponseDto>();

            CreateMap<User, UserInfoDetailsResponseDto>()
                .Include<Patient, PatientInfoResponseDto>()
                .Include<Doctor, DoctorInfoResponseDto>();

            CreateMap<Patient, PatientInfoResponseDto>();
            CreateMap<Doctor, DoctorInfoResponseDto>();
        }
    }
}
