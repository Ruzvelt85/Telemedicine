using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Commands
{
    public class UpdateAppointmentStatusCommandHandler : ICommandHandler<UpdateAppointmentStatusCommand>
    {
        private readonly IReadRepository<Appointment> _appointmentReadRepository;
        private readonly IWriteRepository<Appointment> _appointmentWriteRepository;

        public UpdateAppointmentStatusCommandHandler(
            IReadRepository<Appointment> appointmentReadRepository,
            IWriteRepository<Appointment> appointmentWriteRepository)
        {
            _appointmentReadRepository = appointmentReadRepository;
            _appointmentWriteRepository = appointmentWriteRepository;
        }

        /// <inheritdoc />
        public async Task HandleAsync(UpdateAppointmentStatusCommand command, CancellationToken cancellationToken = default)
        {
            var appointment = await _appointmentReadRepository.GetByIdAsync(command.Id, cancellationToken);
            if (appointment == null)
            {
                throw new EntityNotFoundByIdException(typeof(Appointment), command.Id);
            }

            switch (command.Status)
            {
                case AppointmentStatus.Cancelled:
                    await CancelAppointmentAsync(appointment, command.Reason, cancellationToken);
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        private async Task CancelAppointmentAsync(Appointment appointment, string? reason, CancellationToken cancellationToken)
        {
            var isCancelledSuccessfully = appointment.Cancel(reason);
            if (isCancelledSuccessfully)
            { await _appointmentWriteRepository.UpdateAsync(appointment, cancellationToken); }
        }
    }
}
