using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.MobileClientBffService.API;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public record CreateMoodMeasurementCommand : ICommand<Guid>
    {
        public MoodMeasureType Measure { get; init; }

        public DateTime ClientDate { get; init; }
    }
}
