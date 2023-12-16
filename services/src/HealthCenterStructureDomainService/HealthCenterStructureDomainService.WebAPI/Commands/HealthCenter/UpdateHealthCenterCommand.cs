using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter
{
    public record UpdateHealthCenterCommand : ICommand<Guid>
    {
        public UpdateHealthCenterCommand(string name, string usaState)
        {
            Name = name;
            UsaState = usaState;
        }

        public string Name { get; init; }

        public string UsaState { get; init; }

        public bool IsDeleted { get; init; }
    }
}
