using System;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Commands
{
    public record UpdateAppointmentStatusCommand : ICommand
    {
        public Guid Id { get; init; }

        public string? Reason { get; init; }

        public AppointmentStatus Status { get; init; }
    }
}
