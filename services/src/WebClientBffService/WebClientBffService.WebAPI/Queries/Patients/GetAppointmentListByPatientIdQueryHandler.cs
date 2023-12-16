using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId;
using AppointmentServiceDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients
{
    public class GetAppointmentListByPatientIdQueryHandler : IQueryHandler<GetAppointmentListByPatientIdQuery, AppointmentListByPatientIdResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly ICheckAccessProvider _checkAccessProvider;
        private readonly IUsersQueryService _usersServiceApi;
        private readonly IPatientsQueryService _patientsQueryService;
        private readonly IWebClientBffQueryService _appointmentServiceApi;
        private readonly ICurrentUserProvider _currentUserProvider;

        public GetAppointmentListByPatientIdQueryHandler(IMapper mapper,
            ICheckAccessProvider checkAccessProvider,
            ICurrentUserProvider currentUserProvider,
            IUsersQueryService usersServiceApi,
            IPatientsQueryService patientsQueryService,
            IWebClientBffQueryService appointmentServiceApi)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _checkAccessProvider = checkAccessProvider ?? throw new ArgumentNullException(nameof(checkAccessProvider));
            _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
            _usersServiceApi = usersServiceApi ?? throw new ArgumentNullException(nameof(usersServiceApi));
            _patientsQueryService = patientsQueryService ?? throw new ArgumentNullException(nameof(patientsQueryService));
            _appointmentServiceApi = appointmentServiceApi ?? throw new ArgumentNullException(nameof(appointmentServiceApi));
        }

        /// <inheritdoc />
        public async Task<AppointmentListByPatientIdResponseDto> HandleAsync(GetAppointmentListByPatientIdQuery query, CancellationToken cancellationToken = default)
        {
            // needs to check if there is Patient with such id
            await _patientsQueryService.GetPatientByIdAsync(query.PatientId, cancellationToken);

            await _checkAccessProvider.ShouldHaveSameHealthCenterAsync(query.PatientId, _currentUserProvider.GetId());

            var appointmentsRequest = _mapper.Map<AppointmentServiceDto.AppointmentListRequestDto>(query);
            var appointmentsResponse = await _appointmentServiceApi.GetAppointmentList(appointmentsRequest, cancellationToken);

            IEnumerable<AppointmentResponseDto> items = appointmentsResponse.Items.Any()
                ? await GetAppointmentsWithAttendeesAsync(appointmentsResponse.Items, cancellationToken)
                : Array.Empty<AppointmentResponseDto>();

            return new AppointmentListByPatientIdResponseDto
            {
                Paging = appointmentsResponse.Paging,
                Items = items.ToArray()
            };
        }

        private async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsWithAttendeesAsync(IReadOnlyCollection<AppointmentServiceDto.AppointmentResponseDto> appointments, CancellationToken cancellationToken)
        {
            var attendeeIds = appointments.SelectMany(a => a.Attendees).Distinct().ToArray();

            var attendeesList = await GetAttendeesByIdsAsync(attendeeIds, cancellationToken);

            return MapToAppointmentResponseDto(appointments, attendeesList.OrderBy(x => x.UserType));
        }

        private async Task<List<AttendeeResponseDto>> GetAttendeesByIdsAsync(IEnumerable<Guid> attendeeIds, CancellationToken cancellationToken)
        {
            // TODO: JD-554 заменить на вызов массива Id
            var userInfoTasks = attendeeIds.Select(attendeeId => _usersServiceApi.GetUserInfoAsync(attendeeId, cancellationToken));

            var userInfoResponses = await Task.WhenAll(userInfoTasks);

            return _mapper.Map<List<AttendeeResponseDto>>(userInfoResponses);
        }

        private IEnumerable<AppointmentResponseDto> MapToAppointmentResponseDto(IEnumerable<AppointmentServiceDto.AppointmentResponseDto> appointments, IEnumerable<AttendeeResponseDto> attendeesList)
        {
            AttendeeResponseDto[] GetAttendeeResponseByAppointment(AppointmentServiceDto.AppointmentResponseDto _) =>
                _.Attendees.Join(attendeesList, attendeeId => attendeeId, attendee => attendee.Id, (_, attendee) => attendee).ToArray();

            var appointmentAttendeeList = appointments.ToDictionary(_ => _.Id, GetAttendeeResponseByAppointment);

            var appointmentsResponse = _mapper.Map<IReadOnlyCollection<AppointmentResponseDto>>(appointments);
            appointmentsResponse = _mapper.Map(appointmentAttendeeList, appointmentsResponse);
            return appointmentsResponse;
        }
    }
}
