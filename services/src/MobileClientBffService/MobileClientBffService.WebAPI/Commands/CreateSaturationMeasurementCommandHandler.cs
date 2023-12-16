using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Services.MobileClientBffService.API.Settings;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public class CreateSaturationMeasurementCommandHandler : ICommandHandler<CreateSaturationMeasurementCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISaturationMeasurementCommandService _saturationMeasurementCommandService;
        private readonly IPulseRateMeasurementCommandService _pulseRateMeasurementCommandService;
        private readonly SaturationMeasurementSettings _saturationMeasurementSettings;
        private readonly ILogger _logger = Log.ForContext<CreateSaturationMeasurementCommandHandler>();

        public CreateSaturationMeasurementCommandHandler(IMapper mapper, ICurrentUserProvider currentUserProvider,
            ISaturationMeasurementCommandService saturationMeasurementCommandService, ISaturationMeasurementSettingsBuilder saturationMeasurementSettingsBuilder,
            IPulseRateMeasurementCommandService pulseRateMeasurementCommandService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
            _saturationMeasurementCommandService = saturationMeasurementCommandService ?? throw new ArgumentNullException(nameof(saturationMeasurementCommandService));
            _pulseRateMeasurementCommandService = pulseRateMeasurementCommandService ?? throw new ArgumentNullException(nameof(pulseRateMeasurementCommandService));
            _saturationMeasurementSettings = saturationMeasurementSettingsBuilder?.Build() ?? throw new ArgumentNullException(nameof(saturationMeasurementSettingsBuilder));
        }

        public Task<Guid> HandleAsync(CreateSaturationMeasurementCommand command, CancellationToken cancellationToken = default)
        {
            CreatePulseRate(command, cancellationToken);

            var normalizedCommand = GetNormalizedCommand(command);
            var request = _mapper.Map<CreateMeasurementRequestDto<SaturationMeasurementDto>>(normalizedCommand) with { PatientId = _currentUserProvider.GetId() };
            return _saturationMeasurementCommandService.CreateAsync(request, cancellationToken);
        }

        private CreateSaturationMeasurementCommand GetNormalizedCommand(CreateSaturationMeasurementCommand command)
        {
            // Raw measurement check and trim now here. But if will be other cases, it must be extracted to shared method.
            var maxRawItemsToPassToMeasurementDsCountLimit = _saturationMeasurementSettings.MaxRawItemsToPassToMeasurementDSCountLimit;
            if (command.RawMeasurements is null || command.RawMeasurements.Count <= maxRawItemsToPassToMeasurementDsCountLimit)
            { return command; }

            var limitedRawSaturationItems = command.RawMeasurements!
                .OrderBy(x => x.Order)
                .Take(maxRawItemsToPassToMeasurementDsCountLimit)
                .ToArray();
            return command with { RawMeasurements = limitedRawSaturationItems };
        }

        private void CreatePulseRate(CreateSaturationMeasurementCommand command, CancellationToken cancellationToken)
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
