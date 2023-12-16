using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Exceptions;
using Telemedicine.Services.AppointmentDomainService.Core.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Services;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Commands
{
    [UsedImplicitly]
    public class RescheduleAppointmentCommandHandler : ICommandHandler<RescheduleAppointmentCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<Appointment> _appointmentReadRepository;
        private readonly IWriteRepository<Appointment> _appointmentWriteRepository;
        private readonly IOverlappedAppointmentsService _overlappedAppointmentsService;

        public RescheduleAppointmentCommandHandler(IMapper mapper,
            IReadRepository<Appointment> appointmentReadRepository,
            IWriteRepository<Appointment> appointmentWriteRepository,
            IOverlappedAppointmentsService overlappedAppointmentsService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentReadRepository = appointmentReadRepository ?? throw new ArgumentNullException(nameof(appointmentReadRepository));
            _appointmentWriteRepository = appointmentWriteRepository ?? throw new ArgumentNullException(nameof(appointmentWriteRepository));
            _overlappedAppointmentsService = overlappedAppointmentsService ?? throw new ArgumentNullException(nameof(overlappedAppointmentsService));
        }

        /// <inheritdoc />
        public async Task<Guid> HandleAsync(RescheduleAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            Appointment? appointment = await _appointmentReadRepository.GetByIdAsync(command.Id, cancellationToken);
            if (appointment is null)
            {
                throw new EntityNotFoundByIdException(typeof(Appointment), command.Id);
            }

            Appointment newAppointment = await Reschedule(appointment, command, cancellationToken);
            await _appointmentWriteRepository.UpdateAsync(appointment, cancellationToken);
            await _appointmentWriteRepository.AddAsync(newAppointment, cancellationToken);

            return newAppointment.Id;
        }

        private async Task<Appointment> Reschedule(Appointment appointment, RescheduleAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var dto = _mapper.Map<RescheduleAppointmentDomainDto>(command);
                return await appointment.Reschedule(dto, _overlappedAppointmentsService, cancellationToken);
            }
            catch (Core.Exceptions.AppointmentOverlappedException ex)
            {
                throw new AppointmentOverlappedException(appointment.Id, ex.OverlappedAppointments.ToArray(), ex);
            }
            catch (Core.Exceptions.InvalidAppointmentStateException ex)
            {
                throw new InvalidAppointmentStateException(ex.Message, appointment.Id, (API.Common.Common.AppointmentState)ex.AppointmentState, ex);
            }
        }
    }
}
