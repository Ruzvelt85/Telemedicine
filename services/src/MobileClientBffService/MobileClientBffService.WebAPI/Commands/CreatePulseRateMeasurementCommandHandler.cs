using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public class CreatePulseRateMeasurementCommandHandler : ICommandHandler<CreatePulseRateMeasurementCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPulseRateMeasurementCommandService _pulseRateMeasurementCommandService;

        public CreatePulseRateMeasurementCommandHandler(IMapper mapper, ICurrentUserProvider currentUserProvider, IPulseRateMeasurementCommandService pulseRateMeasurementCommandService)
        {
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _pulseRateMeasurementCommandService = pulseRateMeasurementCommandService;
        }

        public Task<Guid> HandleAsync(CreatePulseRateMeasurementCommand command, CancellationToken cancellationToken = default)
        {
            var request = _mapper.Map<CreateMeasurementRequestDto<PulseRateMeasurementDto>>(command) with { PatientId = _currentUserProvider.GetId() };
            return _pulseRateMeasurementCommandService.CreateAsync(request, cancellationToken);
        }
    }
}
