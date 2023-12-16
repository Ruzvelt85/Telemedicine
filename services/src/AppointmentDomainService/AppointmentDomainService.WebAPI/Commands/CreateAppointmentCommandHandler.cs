using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Exceptions;
using Telemedicine.Services.AppointmentDomainService.Core.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Services;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Commands
{
    public class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentWriteRepository _appointmentWriteRepository;
        private readonly IOverlappedAppointmentsService _overlappedAppointmentsService;

        public CreateAppointmentCommandHandler(IMapper mapper,
            IAppointmentWriteRepository appointmentWriteRepository,
            IOverlappedAppointmentsService overlappedAppointmentsService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentWriteRepository = appointmentWriteRepository ?? throw new ArgumentNullException(nameof(appointmentWriteRepository));
            _overlappedAppointmentsService = overlappedAppointmentsService ?? throw new ArgumentNullException(nameof(overlappedAppointmentsService));
        }

        /// <inheritdoc />
        public async Task<Guid> HandleAsync(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            Appointment newAppointment = await CreateNewAppointment(command, cancellationToken);

            var createdAppointment = await _appointmentWriteRepository.AddAsync(newAppointment, cancellationToken);
            return createdAppointment.Id;
        }

        private async Task<Appointment> CreateNewAppointment(CreateAppointmentCommand command, CancellationToken ct)
        {
            try
            {
                var dto = _mapper.Map<CreateAppointmentDomainDto>(command);
                return await Appointment.Create(dto, _overlappedAppointmentsService, ct);
            }
            catch (Core.Exceptions.AppointmentOverlappedException ex)
            {
                throw new AppointmentOverlappedException(ex.OverlappedAppointments.ToArray(), ex);
            }
        }
    }
}
