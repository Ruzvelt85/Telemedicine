using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentById;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AppointmentsController : BffServiceBaseController, API.Appointments.AppointmentQueryService.IAppointmentQueryService, IAppointmentCommandService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetAppointmentListQuery, AppointmentListResponseDto> _getAppointmentListQueryHandler;
        private readonly IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto> _getAppointmentConnectionInfoQueryHandler;
        private readonly IQueryHandler<GetAppointmentByIdQuery, AppointmentByIdResponseDto> _getAppointmentByIdQueryHandler;
        private readonly ICommandHandler<CreateAppointmentCommand, Guid> _createAppointmentCommandHandler;
        private readonly ICommandHandler<CancelAppointmentCommand> _cancelAppointmentCommandHandler;
        private readonly ICommandHandler<RescheduleAppointmentCommand, Guid> _rescheduleAppointmentCommandHandler;

        public AppointmentsController(IMapper mapper,
            IQueryHandler<GetAppointmentListQuery, AppointmentListResponseDto> getAppointmentListQueryHandler,
            IQueryHandler<GetAppointmentByIdQuery, AppointmentByIdResponseDto> getAppointmentByIdQueryHandler,
            IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto> getAppointmentConnectionInfoQueryHandler,
            ICommandHandler<CreateAppointmentCommand, Guid> createAppointmentCommandHandler,
            ICommandHandler<CancelAppointmentCommand> cancelAppointmentCommandHandler,
            ICommandHandler<RescheduleAppointmentCommand, Guid> rescheduleAppointmentCommandHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getAppointmentListQueryHandler = getAppointmentListQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentListQueryHandler));
            _getAppointmentByIdQueryHandler = getAppointmentByIdQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentByIdQueryHandler));
            _createAppointmentCommandHandler = createAppointmentCommandHandler ?? throw new ArgumentNullException(nameof(createAppointmentCommandHandler));
            _getAppointmentConnectionInfoQueryHandler = getAppointmentConnectionInfoQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentConnectionInfoQueryHandler));
            _cancelAppointmentCommandHandler = cancelAppointmentCommandHandler ?? throw new ArgumentNullException(nameof(cancelAppointmentCommandHandler));
            _rescheduleAppointmentCommandHandler = rescheduleAppointmentCommandHandler ?? throw new ArgumentNullException(nameof(rescheduleAppointmentCommandHandler));
        }

        [HttpGet]
        public async Task<AppointmentListResponseDto> GetAppointmentList([FromQuery] AppointmentListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetAppointmentListQuery>(request);
            AppointmentListResponseDto appointmentList = await _getAppointmentListQueryHandler.HandleAsync(query, cancellationToken);
            return appointmentList;
        }

        [HttpGet("{id}")]
        public async Task<AppointmentByIdResponseDto> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _getAppointmentByIdQueryHandler.HandleAsync(new GetAppointmentByIdQuery(id), cancellationToken);
            return result;
        }

        [HttpPost]
        public async Task<Guid> CreateAppointment([FromBody] CreateAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            CreateAppointmentCommand command = _mapper.Map<CreateAppointmentCommand>(request);
            Guid id = await _createAppointmentCommandHandler.HandleAsync(command, cancellationToken);
            return id;
        }

        [HttpPut("cancel")]
        public async Task CancelAppointment([FromBody] CancelAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CancelAppointmentCommand>(request);
            await _cancelAppointmentCommandHandler.HandleAsync(command, cancellationToken);
        }

        [HttpGet("connectioninfo/{id}")]
        public async Task<AppointmentConnectionInfoResponseDto> GetAppointmentConnectionInfoAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _getAppointmentConnectionInfoQueryHandler.HandleAsync(new GetAppointmentConnectionInfoQuery(id), cancellationToken);
            return result;
        }

        [HttpPut("{id}/reschedule")]
        public async Task<Guid> Reschedule(Guid id, [FromBody] RescheduleAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            RescheduleAppointmentCommand command = _mapper.Map<RescheduleAppointmentCommand>(request) with { Id = id };
            return await _rescheduleAppointmentCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}
