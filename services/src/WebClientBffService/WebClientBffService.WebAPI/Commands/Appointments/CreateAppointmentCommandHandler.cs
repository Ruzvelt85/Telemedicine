using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments
{
    public class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ICheckAccessProvider _checkAccessProvider;
        private readonly IAppointmentCommandService _appointmentCommandService;

        public CreateAppointmentCommandHandler(IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            ICheckAccessProvider checkAccessProvider,
            IAppointmentCommandService appointmentCommandService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
            _checkAccessProvider = checkAccessProvider ?? throw new ArgumentNullException(nameof(checkAccessProvider));
            _appointmentCommandService = appointmentCommandService ?? throw new ArgumentNullException(nameof(appointmentCommandService));
        }

        public async Task<Guid> HandleAsync(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
        {
            await _checkAccessProvider.ShouldHaveSameHealthCenterAsync(command.AttendeeIds);

            var request = _mapper.Map<CreateAppointmentRequestDto>(command) with { CreatorId = _currentUserProvider.GetId() };

            var appointmentId = await _appointmentCommandService.CreateAppointment(request, cancellationToken);
            return appointmentId;
        }
    }
}
