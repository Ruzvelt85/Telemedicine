using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments
{
    [UsedImplicitly]
    public class RescheduleAppointmentCommandHandler : ICommandHandler<RescheduleAppointmentCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentCommandService _appointmentCommandService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public RescheduleAppointmentCommandHandler(IMapper mapper, IAppointmentCommandService appointmentCommandService, ICurrentUserProvider currentUserProvider)
        {
            _mapper = mapper;
            _appointmentCommandService = appointmentCommandService;
            _currentUserProvider = currentUserProvider;
        }

        /// <inheritdoc />
        public Task<Guid> HandleAsync(RescheduleAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            var request = _mapper.Map<RescheduleAppointmentRequestDto>(command) with { CreatorId = _currentUserProvider.GetId() };
            return _appointmentCommandService.Reschedule(command.Id, request, cancellationToken);
        }
    }
}
