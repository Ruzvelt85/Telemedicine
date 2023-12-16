using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentById;
using Serilog;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments
{
    public class GetAppointmentByIdQueryHandler : IQueryHandler<GetAppointmentByIdQuery, AppointmentByIdResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentQueryService _appointmentQueryService;
        private readonly IUsersQueryService _usersQueryService;
        private readonly ILogger _logger = Log.ForContext<GetAppointmentByIdQueryHandler>();

        public GetAppointmentByIdQueryHandler(IMapper mapper, IAppointmentQueryService appointmentQueryService, IUsersQueryService usersQueryService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentQueryService = appointmentQueryService ?? throw new ArgumentNullException(nameof(appointmentQueryService));
            _usersQueryService = usersQueryService ?? throw new ArgumentNullException(nameof(usersQueryService));
        }

        /// <inheritdoc />
        public async Task<AppointmentByIdResponseDto> HandleAsync(GetAppointmentByIdQuery query, CancellationToken cancellationToken = default)
        {
            var appointmentByIdResponseDto = await _appointmentQueryService.GetAppointmentByIdAsync(query.Id, cancellationToken);

            var attendees = await GetAttendeesByIds(appointmentByIdResponseDto.Attendees, cancellationToken);

            return _mapper.Map<AppointmentByIdResponseDto>(appointmentByIdResponseDto) with { Attendees = attendees };
        }

        private async Task<List<AttendeeResponseDto>> GetAttendeesByIds(IReadOnlyCollection<Guid> attendeeIds, CancellationToken cancellationToken)
        {
            // TODO: JD-554 заменить на вызов массива Id
            var userInfoTasks = attendeeIds.Select(attendeeId => _usersQueryService.GetUserInfoDetailsAsync(attendeeId, cancellationToken));

            UserInfoDetailsResponseDto[] userInfoResponses;
            try
            {
                userInfoResponses = await Task.WhenAll(userInfoTasks);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Error(ex, "Message: {Message}", ex.Message);
                throw;
            }

            return _mapper.Map<List<AttendeeResponseDto>>(userInfoResponses);
        }
    }
}
