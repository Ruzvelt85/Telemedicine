using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Commands;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Queries;
using Microsoft.AspNetCore.Mvc;
using AppointmentListRequestDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentListRequestDto;
using AppointmentListResponseDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto.AppointmentListResponseDto;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AppointmentsController : DomainServiceBaseController, IWebClientBffQueryService, IMobileClientBffQueryService, IAppointmentCommandService, IAppointmentQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<Queries.WebClient.GetAppointmentListQuery, AppointmentListResponseDto> _getAppointmentListQueryHandler;
        private readonly IQueryHandler<GetAppointmentByIdQuery, AppointmentByIdResponseDto> _getAppointmentByIdQueryHandler;
        private readonly IQueryHandler<Queries.WebClient.NearestAppointmentsByAttendeeQuery, NearestAppointmentsResponseDto> _getNearestAppointmentsQueryHandler;
        private readonly IQueryHandler<Queries.MobileClient.GetAppointmentListQuery, API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto> _getAppointmentListForMobileClientQueryHandler;
        private readonly IQueryHandler<Queries.MobileClient.GetChangedAppointmentListQuery, API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto> _getChangedAppointmentListQueryHandler;
        private readonly ICommandHandler<UpdateAppointmentStatusCommand> _updateAppointmentStatusCommandHandler;
        private readonly ICommandHandler<CreateAppointmentCommand, Guid> _createAppointmentCommandHandler;
        private readonly ICommandHandler<RescheduleAppointmentCommand, Guid> _rescheduleAppointmentCommandHandler;

        public AppointmentsController(IMapper mapper,
            IQueryHandler<Queries.WebClient.GetAppointmentListQuery, AppointmentListResponseDto> getAppointmentListQueryHandler,
            IQueryHandler<GetAppointmentByIdQuery, AppointmentByIdResponseDto> getAppointmentByIdQueryHandler,
            IQueryHandler<Queries.WebClient.NearestAppointmentsByAttendeeQuery, NearestAppointmentsResponseDto> getNearestAppointmentsQueryHandler,
            IQueryHandler<Queries.MobileClient.GetAppointmentListQuery, API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto> getAppointmentListForMobileClientQueryHandler,
            IQueryHandler<Queries.MobileClient.GetChangedAppointmentListQuery, API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto> getChangedAppointmentListQueryHandler,
            ICommandHandler<UpdateAppointmentStatusCommand> updateAppointmentStatusCommandHandler,
            ICommandHandler<CreateAppointmentCommand, Guid> createAppointmentCommandHandler,
            ICommandHandler<RescheduleAppointmentCommand, Guid> rescheduleAppointmentCommandHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getAppointmentListQueryHandler = getAppointmentListQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentListQueryHandler));
            _getAppointmentByIdQueryHandler = getAppointmentByIdQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentByIdQueryHandler));
            _getNearestAppointmentsQueryHandler = getNearestAppointmentsQueryHandler ?? throw new ArgumentNullException(nameof(getNearestAppointmentsQueryHandler));
            _getAppointmentListForMobileClientQueryHandler = getAppointmentListForMobileClientQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentListForMobileClientQueryHandler));
            _getChangedAppointmentListQueryHandler = getChangedAppointmentListQueryHandler ?? throw new ArgumentNullException(nameof(getChangedAppointmentListQueryHandler));
            _updateAppointmentStatusCommandHandler = updateAppointmentStatusCommandHandler ?? throw new ArgumentNullException(nameof(updateAppointmentStatusCommandHandler));
            _createAppointmentCommandHandler = createAppointmentCommandHandler ?? throw new ArgumentNullException(nameof(createAppointmentCommandHandler));
            _rescheduleAppointmentCommandHandler = rescheduleAppointmentCommandHandler ?? throw new ArgumentNullException(nameof(rescheduleAppointmentCommandHandler));
        }


        [HttpGet("/api/web/[controller]")]
        public async Task<AppointmentListResponseDto> GetAppointmentList([FromQuery] AppointmentListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<Queries.WebClient.GetAppointmentListQuery>(request);
            var appointments = await _getAppointmentListQueryHandler.HandleAsync(query, cancellationToken);
            return appointments;
        }

        [HttpGet("{id}")]
        public async Task<AppointmentByIdResponseDto> GetAppointmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _getAppointmentByIdQueryHandler.HandleAsync(new GetAppointmentByIdQuery(id), cancellationToken);
            return result;
        }

        [HttpGet("nearest")]
        public Task<NearestAppointmentsResponseDto> GetNearestAppointmentsByAttendeeIdAsync(Guid attendeeId, CancellationToken cancellationToken = default)
        {
            return _getNearestAppointmentsQueryHandler.HandleAsync(new Queries.WebClient.NearestAppointmentsByAttendeeQuery(attendeeId), cancellationToken);
        }

        [HttpGet("/api/mobile/[controller]")]
        public async Task<API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto> GetAppointmentList([FromQuery] API.Common.MobileClientBffQueryService.Dto.AppointmentListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<Queries.MobileClient.GetAppointmentListQuery>(request);
            var appointments = await _getAppointmentListForMobileClientQueryHandler.HandleAsync(query, cancellationToken);
            return appointments;
        }

        [HttpGet("/api/mobile/[controller]/changed")]
        public Task<API.Common.MobileClientBffQueryService.Dto.AppointmentListResponseDto> GetChangedAppointmentList([FromQuery] ChangedAppointmentListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<Queries.MobileClient.GetChangedAppointmentListQuery>(request);
            return _getChangedAppointmentListQueryHandler.HandleAsync(query, cancellationToken);
        }

        //NOW: change to CancelAppointment
        [HttpPut("status")]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task UpdateStatus([FromBody] UpdateAppointmentStatusRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<UpdateAppointmentStatusCommand>(request);
            await _updateAppointmentStatusCommandHandler.HandleAsync(command, cancellationToken);
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public Task<Guid> CreateAppointment([FromBody] CreateAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateAppointmentCommand>(request);
            return _createAppointmentCommandHandler.HandleAsync(command, cancellationToken);
        }

        [HttpPut("{id}/reschedule")]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public Task<Guid> Reschedule(Guid id, [FromBody] RescheduleAppointmentRequestDto request, CancellationToken cancellationToken = default)
        {
            RescheduleAppointmentCommand command = _mapper.Map<RescheduleAppointmentCommand>(request) with { Id = id };
            return _rescheduleAppointmentCommandHandler.HandleAsync(command, cancellationToken);
        }
    }
}
