using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter
{
    public record CreateHealthCenterCommand : ICommand<Guid>
    {
        public CreateHealthCenterCommand(string innerId, string name, string usaState)
        {
            InnerId = innerId;
            Name = name;
            UsaState = usaState;
        }

        public string InnerId { get; init; }

        public string Name { get; init; }

        public string UsaState { get; init; }

        // TODO: JD-1282 - Remove this property from CreateCommand
        public bool IsDeleted { get; init; }
    }
}
