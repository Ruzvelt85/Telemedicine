using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthMeasurementsController : BffServiceBaseController, IHealthMeasurementCommandService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateMoodMeasurementCommand, Guid> _createMoodMeasurementCommandHandler;
        private readonly ICommandHandler<CreateBloodPressureMeasurementCommand, Guid> _createBloodPressureMeasurementCommandHandler;
        private readonly ICommandHandler<CreateSaturationMeasurementCommand, Guid> _createSaturationMeasurementCommandHandler;
        private readonly ICommandHandler<CreatePulseRateMeasurementCommand, Guid> _createPulseRateMeasurementCommandHandler;

        public HealthMeasurementsController(IMapper mapper, ICommandHandler<CreateMoodMeasurementCommand, Guid> createMoodMeasurementCommandHandler,
            ICommandHandler<CreateBloodPressureMeasurementCommand, Guid> createBloodPressureMeasurementCommandHandler,
            ICommandHandler<CreateSaturationMeasurementCommand, Guid> createSaturationMeasurementCommandHandler,
            ICommandHandler<CreatePulseRateMeasurementCommand, Guid> createPulseRateMeasurementCommandHandler)
        {
            _mapper = mapper;
            _createMoodMeasurementCommandHandler = createMoodMeasurementCommandHandler;
            _createBloodPressureMeasurementCommandHandler = createBloodPressureMeasurementCommandHandler;
            _createSaturationMeasurementCommandHandler = createSaturationMeasurementCommandHandler;
            _createPulseRateMeasurementCommandHandler = createPulseRateMeasurementCommandHandler;
        }

        [HttpPost("moods")]
        public Task<Guid> CreateMoodMeasurementAsync([FromBody] CreateMoodMeasurementRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateMoodMeasurementCommand>(request);
            return _createMoodMeasurementCommandHandler.HandleAsync(command, cancellationToken);
        }

        [HttpPost("bloodpressures")]
        public Task<Guid> CreateBloodPressureMeasurementAsync([FromBody] CreateBloodPressureMeasurementRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateBloodPressureMeasurementCommand>(request);
            return _createBloodPressureMeasurementCommandHandler.HandleAsync(command, cancellationToken);
        }

        [HttpPost("saturations")]
        public Task<Guid> CreateSaturationMeasurementAsync(CreateSaturationMeasurementRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateSaturationMeasurementCommand>(request);
            return _createSaturationMeasurementCommandHandler.HandleAsync(command, cancellationToken);
        }

        [HttpPost("pulserate")]
        public Task<Guid> CreatePulseRateMeasurementAsync(CreatePulseRateMeasurementRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreatePulseRateMeasurementCommand>(request);
            return _createPulseRateMeasurementCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}
