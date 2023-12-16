using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public record CreatePulseRateMeasurementCommand : ICommand<Guid>
    {
        public int PulseRate { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
