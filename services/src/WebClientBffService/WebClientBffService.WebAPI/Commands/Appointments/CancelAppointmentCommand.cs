using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments
{
    public class CancelAppointmentCommand : ICommand
    {
        public CancelAppointmentCommand(Guid id, string reason)
        {
            Id = id;
            Reason = reason;
        }

        public Guid Id { get; init; }

        public string Reason { get; init; }
    }
}
