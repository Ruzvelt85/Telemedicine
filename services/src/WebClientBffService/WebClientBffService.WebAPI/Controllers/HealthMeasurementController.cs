using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class HealthMeasurementController : ServiceBaseController, IHealthMeasurementQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetHealthMeasurementListQuery, MeasurementListResponseDto> _getHealthMeasurementListQueryHandler;

        public HealthMeasurementController(IMapper mapper,
            IQueryHandler<GetHealthMeasurementListQuery, MeasurementListResponseDto> getHealthMeasurementListQueryHandler)
        {
            _mapper = mapper;
            _getHealthMeasurementListQueryHandler = getHealthMeasurementListQueryHandler;
        }

        [HttpGet]
        public async Task<MeasurementListResponseDto> GetMeasurementList([FromQuery] GetMeasurementListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetHealthMeasurementListQuery>(request);
            var measurementList = await _getHealthMeasurementListQueryHandler.HandleAsync(query, cancellationToken);
            return measurementList;
        }
    }
}
