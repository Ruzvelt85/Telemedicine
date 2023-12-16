using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands
{
    public class CreatePulseRateMeasurementCommandHandler : ICommandHandler<CreateMeasurementCommand<PulseRateMeasurementDto>, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IPulseRateMeasurementWriteRepository _pulseRateMeasurementWriteRepository;

        public CreatePulseRateMeasurementCommandHandler(IMapper mapper, IPulseRateMeasurementWriteRepository pulseRateMeasurementWriteRepository)
        {
            _mapper = mapper;
            _pulseRateMeasurementWriteRepository = pulseRateMeasurementWriteRepository;
        }

        /// <inheritdoc />
        public async Task<Guid> HandleAsync(CreateMeasurementCommand<PulseRateMeasurementDto> command, CancellationToken cancellationToken = default)
        {
            var pulseRateMeasurement = _mapper.Map<PulseRateMeasurement>(command);
            PulseRateMeasurement result = await _pulseRateMeasurementWriteRepository.AddAsync(pulseRateMeasurement, cancellationToken);
            return result.Id;
        }
    }
}
