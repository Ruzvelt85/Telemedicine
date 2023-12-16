using AutoMapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands
{
    public class CreateSaturationMeasurementCommandHandler : ICommandHandler<CreateMeasurementCommand<SaturationMeasurementDto>, Guid>
    {
        private readonly IMapper _mapper;
        private readonly ISaturationMeasurementWriteRepository _saturationMeasurementWriteRepository;
        private readonly ILogger _logger = Log.ForContext(typeof(CreateSaturationMeasurementCommandHandler));

        private const string CollectionHasDuplicatesLogMsg = "Collection of {RawSaturationItemCommandDto} in {CreateSaturationMeasurementCommand} contains items with a duplicate order." +
                                                             " This might indicate incorrectness of the received data.";

        public CreateSaturationMeasurementCommandHandler(IMapper mapper, ISaturationMeasurementWriteRepository saturationMeasurementWriteRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _saturationMeasurementWriteRepository = saturationMeasurementWriteRepository ?? throw new ArgumentNullException(nameof(saturationMeasurementWriteRepository));
        }

        /// <inheritdoc />
        public async Task<Guid> HandleAsync(CreateMeasurementCommand<SaturationMeasurementDto> command, CancellationToken cancellationToken = default)
        {
            if (command.Measure.RawMeasurements is not null)
            { CheckIsUniqueOrderAndLog(command.Measure.RawMeasurements); }

            var saturationMeasurement = _mapper.Map<SaturationMeasurement>(command);
            SaturationMeasurement result = await _saturationMeasurementWriteRepository.AddAsync(saturationMeasurement, cancellationToken);
            return result.Id;
        }

        private void CheckIsUniqueOrderAndLog(IReadOnlyCollection<RawSaturationMeasurementItemDto> rawMeasurements)
        {
            var uniqueOrderCount = rawMeasurements
                .Select(_ => _.Order)
                .Distinct()
                .Count();

            if (rawMeasurements.Count != uniqueOrderCount)
            {
                _logger.Warning(CollectionHasDuplicatesLogMsg, nameof(RawSaturationMeasurementItemDto), nameof(CreateMeasurementCommand<SaturationMeasurementDto>));
            }
        }
    }
}
