using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodPressureMeasurementsController : DomainServiceBaseController, IBloodPressureMeasurementCommandService, IBloodPressureMeasurementQueryService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateMeasurementCommand<BloodPressureMeasurementDto>, Guid> _createBloodPressureMeasurementCommandHandler;
        private readonly IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>> _getBloodPressureMeasurementListQueryHandler;

        public BloodPressureMeasurementsController(IMapper mapper,
            ICommandHandler<CreateMeasurementCommand<BloodPressureMeasurementDto>, Guid> createBloodPressureMeasurementCommandHandler,
            IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>> getBloodPressureMeasurementListQueryHandler)
        {
            _mapper = mapper;
            _createBloodPressureMeasurementCommandHandler = createBloodPressureMeasurementCommandHandler;
            _getBloodPressureMeasurementListQueryHandler = getBloodPressureMeasurementListQueryHandler;
        }

        [HttpGet]
        public async Task<PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>> GetBloodPressureList(
            [FromQuery] GetMeasurementListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetMeasurementListQuery>(request);
            var result = await _getBloodPressureMeasurementListQueryHandler.HandleAsync(query, cancellationToken);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid> CreateAsync([FromBody] CreateMeasurementRequestDto<BloodPressureMeasurementDto> request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateMeasurementCommand<BloodPressureMeasurementDto>>(request);
            var result = await _createBloodPressureMeasurementCommandHandler.HandleAsync(command, cancellationToken);
            return result;
        }
    }
}
