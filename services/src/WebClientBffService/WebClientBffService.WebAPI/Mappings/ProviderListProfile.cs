using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Providers;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Mappings
{
    public class ProviderListProfile : Profile
    {
        public ProviderListProfile()
        {
            CreateMap<ProviderListRequestDto, GetProviderListQuery>();

            CreateMap<ProviderListFilterRequestDto, DoctorListFilterRequestDto>();

            CreateMap<GetProviderListQuery, DoctorListRequestDto>();

            CreateMap<DoctorResponseDto, ProviderResponseDto>();
            CreateMap<PagedListResponseDto<DoctorResponseDto>, PagedListResponseDto<ProviderResponseDto>>();
        }
    }
}
