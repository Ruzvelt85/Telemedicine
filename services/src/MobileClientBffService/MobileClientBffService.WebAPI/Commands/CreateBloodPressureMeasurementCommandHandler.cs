using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public class CreateBloodPressureMeasurementCommandHandler : ICommandHandler<CreateBloodPressureMeasurementCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IBloodPressureMeasurementCommandService _bloodPressureMeasurementCommandService;
        private readonly IPulseRateMeasurementCommandService _pulseRateMeasurementCommandService;
        private readonly ILogger _logger = Log.ForContext<CreateBloodPressureMeasurementCommandHandler>();

        public CreateBloodPressureMeasurementCommandHandler(IMapper mapper, ICurrentUserProvider currentUserProvider, IBloodPressureMeasurementCommandService bloodPressureMeasurementCommandService, IPulseRateMeasurementCommandService pulseRateMeasurementCommandService)
        {
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _bloodPressureMeasurementCommandService = bloodPressureMeasurementCommandService;
            _pulseRateMeasurementCommandService = pulseRateMeasurementCommandService;
        }

        public Task<Guid> HandleAsync(CreateBloodPressureMeasurementCommand command, CancellationToken cancellationToken = default)
        {
            CreatePulseRate(command, cancellationToken);

            var createBloodPressureRequest = _mapper.Map<CreateMeasurementRequestDto<BloodPressureMeasurementDto>>(command) with { PatientId = _currentUserProvider.GetId() };
            return _bloodPressureMeasurementCommandService.CreateAsync(createBloodPressureRequest, cancellationToken);
        }

        private void CreatePulseRate(CreateBloodPressureMeasurementCommand command, CancellationToken cancellationToken)
        {
            var createPulseRequest = _mapper.Map<CreateMeasurementRequestDto<PulseRateMeasurementDto>>(command) with { PatientId = _currentUserProvider.GetId() };

            // here we do not await creating pulse rate, but still want to know about errors
            _pulseRateMeasurementCommandService.CreateAsync(createPulseRequest, cancellationToken).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.Error(t.Exception, "Error occurred while creating pulse rate measurement in CreateBloodPressureMeasurementCommandHandler");
                }

                return Task.CompletedTask;
            }, cancellationToken);
        }
    }
}
