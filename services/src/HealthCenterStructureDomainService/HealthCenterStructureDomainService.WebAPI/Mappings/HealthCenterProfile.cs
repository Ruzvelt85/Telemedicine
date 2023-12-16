using AutoMapper;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Mappings
{
    public class HealthCenterProfile : Profile
    {
        public HealthCenterProfile()
        {
            CreateMap<CreateOrUpdateHealthCenterRequestDto, CreateOrUpdateHealthCenterCommand>();

            CreateMap<CreateOrUpdateHealthCenterCommand, CreateHealthCenterCommand>(MemberList.Destination)
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsActive.HasValue && !src.IsActive.Value));

            CreateMap<CreateOrUpdateHealthCenterCommand, UpdateHealthCenterCommand>(MemberList.Destination)
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsActive.HasValue && !src.IsActive.Value));

            CreateMap<CreateHealthCenterCommand, HealthCenter>(MemberList.Source);
            CreateMap<UpdateHealthCenterCommand, HealthCenter>(MemberList.Source)
                .ForMember(dest => dest.InnerId, opt => opt.UseDestinationValue());

            CreateMap<HealthCenter, HealthCenterResponseDto>();
        }
    }
}
