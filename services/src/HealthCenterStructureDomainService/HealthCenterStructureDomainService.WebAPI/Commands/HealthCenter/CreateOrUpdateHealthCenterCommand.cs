using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter
{
    public record CreateOrUpdateHealthCenterCommand : ICommand<Guid?>
    {
        public CreateOrUpdateHealthCenterCommand(string innerId, string name, string usaState, bool? isActive)
        {
            InnerId = innerId;
            Name = name;
            UsaState = usaState;
            IsActive = isActive;
        }

        public string InnerId { get; init; }

        public string Name { get; init; }

        public string UsaState { get; init; }

        public bool? IsActive { get; init; }
    }
}
