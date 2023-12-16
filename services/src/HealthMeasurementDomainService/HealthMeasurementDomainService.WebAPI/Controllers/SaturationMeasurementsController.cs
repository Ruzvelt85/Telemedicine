using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaturationMeasurementsController : DomainServiceBaseController, ISaturationMeasurementQueryService, ISaturationMeasurementCommandService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateMeasurementCommand<SaturationMeasurementDto>, Guid> _createSaturationMeasurementCommandHandler;
        private readonly IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>> _getSaturationMeasurementListQueryHandler;

        public SaturationMeasurementsController(IMapper mapper,
            ICommandHandler<CreateMeasurementCommand<SaturationMeasurementDto>, Guid> createSaturationMeasurementCommandHandler,
            IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>> getSaturationMeasurementListQueryHandler)
        {
            _mapper = mapper;
            _getSaturationMeasurementListQueryHandler = getSaturationMeasurementListQueryHandler;
            _createSaturationMeasurementCommandHandler = createSaturationMeasurementCommandHandler;
        }

        [HttpGet]
        public async Task<PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>> GetSaturationList(
            [FromQuery] GetMeasurementListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetMeasurementListQuery>(request);
            var result = await _getSaturationMeasurementListQueryHandler.HandleAsync(query, cancellationToken);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid> CreateAsync([FromBody] CreateMeasurementRequestDto<SaturationMeasurementDto> request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateMeasurementCommand<SaturationMeasurementDto>>(request);
            var newSaturationMeasurementId = await _createSaturationMeasurementCommandHandler.HandleAsync(command, cancellationToken);
            return newSaturationMeasurementId;
        }
    }
}
