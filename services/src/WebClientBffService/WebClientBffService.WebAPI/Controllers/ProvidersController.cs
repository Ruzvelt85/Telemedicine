using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Providers;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Controllers
{
    [Route("api/users/[controller]")]
    [Route("api/[controller]")] // TODO: Sapegin, JD-1377 - Remove old path
    public class ProvidersController : BffServiceBaseController, IProviderQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetProviderListQuery, PagedListResponseDto<ProviderResponseDto>> _getProviderListQueryHandler;

        public ProvidersController(IMapper mapper,
            IQueryHandler<GetProviderListQuery, PagedListResponseDto<ProviderResponseDto>> getProviderListQueryHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getProviderListQueryHandler = getProviderListQueryHandler ?? throw new ArgumentNullException(nameof(getProviderListQueryHandler));
        }

        [HttpGet]
        //[Authorize(Policy = "Admin")] // TODO: Sapegin,  JD-1374 - Add security policy for Admin
        public async Task<PagedListResponseDto<ProviderResponseDto>> GetProviderListAsync([FromQuery] ProviderListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetProviderListQuery>(request);
            PagedListResponseDto<ProviderResponseDto> providerList = await _getProviderListQueryHandler.HandleAsync(query, cancellationToken);
            return providerList;
        }
    }
}
