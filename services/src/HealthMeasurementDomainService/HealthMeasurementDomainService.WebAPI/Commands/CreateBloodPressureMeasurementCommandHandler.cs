using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands
{
    public class CreateBloodPressureMeasurementCommandHandler : ICommandHandler<CreateMeasurementCommand<BloodPressureMeasurementDto>, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IBloodPressureMeasurementWriteRepository _bloodPressureMeasurementWriteRepository;

        public CreateBloodPressureMeasurementCommandHandler(IMapper mapper, IBloodPressureMeasurementWriteRepository bloodPressureMeasurementWriteRepository)
        {
            _mapper = mapper;
            _bloodPressureMeasurementWriteRepository = bloodPressureMeasurementWriteRepository;
        }

        /// <inheritdoc />
        public async Task<Guid> HandleAsync(CreateMeasurementCommand<BloodPressureMeasurementDto> command, CancellationToken cancellationToken = default)
        {
            var bloodPressureMeasurement = _mapper.Map<BloodPressureMeasurement>(command);
            BloodPressureMeasurement result = await _bloodPressureMeasurementWriteRepository.AddAsync(bloodPressureMeasurement, cancellationToken);
            return result.Id;
        }
    }
}
