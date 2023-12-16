using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : BffServiceBaseController, IMobileClientQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetLastChangedDataQuery, LastChangedDataResponseDto> _getLastChangedDataQueryHandler;

        public DataController(IMapper mapper, IQueryHandler<GetLastChangedDataQuery, LastChangedDataResponseDto> getLastChangedDataQueryHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getLastChangedDataQueryHandler = getLastChangedDataQueryHandler ?? throw new ArgumentNullException(nameof(getLastChangedDataQueryHandler));
        }

        [HttpGet]
        public Task<LastChangedDataResponseDto> GetLastChangedData([FromQuery] LastChangedDataRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetLastChangedDataQuery>(request);
            return _getLastChangedDataQueryHandler.HandleAsync(query, cancellationToken);
        }
    }
}
