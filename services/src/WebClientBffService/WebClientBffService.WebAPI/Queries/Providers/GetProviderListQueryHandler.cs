using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Providers
{
    public class GetProviderListQueryHandler : IQueryHandler<GetProviderListQuery, PagedListResponseDto<ProviderResponseDto>>
    {
        private readonly IDoctorsQueryService _paceStructureService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IMapper _mapper;

        public GetProviderListQueryHandler(IMapper mapper, ICurrentUserProvider currentUserProvider,
            IDoctorsQueryService paceStructureService)
        {
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _paceStructureService = paceStructureService;
        }

        public async Task<PagedListResponseDto<ProviderResponseDto>> HandleAsync(GetProviderListQuery query, CancellationToken cancellationToken = default)
        {
            // TODO: Sapegin, JD-1507 - Add validation of access of admin to specified health centers
            // Throw AccessDeniedException (child of SecurityException - that is child of ServiceLayerException)

            var doctorsRequest = _mapper.Map<DoctorListRequestDto>(query);
            var doctorsResponse = await _paceStructureService.GetDoctorListAsync(doctorsRequest, cancellationToken);

            return _mapper.Map<PagedListResponseDto<ProviderResponseDto>>(doctorsResponse);
        }
    }
}
