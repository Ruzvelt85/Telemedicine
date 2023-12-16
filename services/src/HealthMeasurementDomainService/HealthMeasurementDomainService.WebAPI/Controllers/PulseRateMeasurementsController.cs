using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PulseRateMeasurementsController : DomainServiceBaseController, IPulseRateMeasurementQueryService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateMeasurementCommand<PulseRateMeasurementDto>, Guid> _createPulseRateMeasurementCommandHandler;
        private readonly IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>> _getPulseRateMeasurementListQueryHandler;

        public PulseRateMeasurementsController(IMapper mapper,
            ICommandHandler<CreateMeasurementCommand<PulseRateMeasurementDto>, Guid> createPulseRateMeasurementCommandHandler,
            IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>> getPulseRateMeasurementListQueryHandler)
        {
            _mapper = mapper;
            _createPulseRateMeasurementCommandHandler = createPulseRateMeasurementCommandHandler;
            _getPulseRateMeasurementListQueryHandler = getPulseRateMeasurementListQueryHandler;
        }

        [HttpGet]
        public async Task<PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>> GetPulseRateList(
            [FromQuery] GetMeasurementListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetMeasurementListQuery>(request);
            var result = await _getPulseRateMeasurementListQueryHandler.HandleAsync(query, cancellationToken);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid> CreateAsync([FromBody] CreateMeasurementRequestDto<PulseRateMeasurementDto> request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateMeasurementCommand<PulseRateMeasurementDto>>(request);
            var result = await _createPulseRateMeasurementCommandHandler.HandleAsync(command, cancellationToken);
            return result;
        }
    }
}
