using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments
{
    public class CancelAppointmentCommandHandler : ICommandHandler<CancelAppointmentCommand>
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentCommandService _appointmentCommandService;

        public CancelAppointmentCommandHandler(IMapper mapper, IAppointmentCommandService appointmentCommandService)
        {
            _mapper = mapper;
            _appointmentCommandService = appointmentCommandService;
        }

        /// <inheritdoc />
        public async Task HandleAsync(CancelAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var request = _mapper.Map<UpdateAppointmentStatusRequestDto>(command);
            await _appointmentCommandService.UpdateStatus(request, cancellationToken);
        }
    }
}
