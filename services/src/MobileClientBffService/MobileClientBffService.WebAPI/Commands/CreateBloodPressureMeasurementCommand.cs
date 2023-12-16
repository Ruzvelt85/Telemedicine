using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public record CreateBloodPressureMeasurementCommand : ICommand<Guid>
    {
        public int Systolic { get; init; }

        public int Diastolic { get; init; }

        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
