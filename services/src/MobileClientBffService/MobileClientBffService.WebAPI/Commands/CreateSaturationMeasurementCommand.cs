using System;
using System.Collections.Generic;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public record CreateSaturationMeasurementCommand : ICommand<Guid>
    {
        public int SpO2 { get; init; }

        public int PulseRate { get; init; }

        public decimal Pi { get; init; }

        public IReadOnlyCollection<RawSaturationItemCommandDto>? RawMeasurements { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
