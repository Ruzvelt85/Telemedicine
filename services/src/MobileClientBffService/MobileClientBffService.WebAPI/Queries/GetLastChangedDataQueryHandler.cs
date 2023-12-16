using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using AppointmentDomainServiceDto = Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Queries
{
    public class GetLastChangedDataQueryHandler : IQueryHandler<GetLastChangedDataQuery, LastChangedDataResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IUsersQueryService _usersQueryService;
        private readonly IMobileClientBffQueryService _appointmentService;
        private readonly IMoodMeasurementQueryService _moodMeasurementService;

        public GetLastChangedDataQueryHandler(IMapper mapper,
            ICurrentUserProvider currentUserProvider,
            IUsersQueryService usersQueryService,
            IMobileClientBffQueryService appointmentService,
            IMoodMeasurementQueryService moodMeasurementService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
            _usersQueryService = usersQueryService ?? throw new ArgumentNullException(nameof(usersQueryService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _moodMeasurementService = moodMeasurementService ?? throw new ArgumentNullException(nameof(moodMeasurementService));
        }

        /// <inheritdoc />
        public async Task<LastChangedDataResponseDto> HandleAsync(GetLastChangedDataQuery query, CancellationToken cancellationToken = default)
        {
            var appointmentsDataTask = GetAppointments(query.AppointmentsLastUpdate, cancellationToken);
            var lastChangedMoodTask = GetLastChangedMood(query, cancellationToken);

            await Task.WhenAll(appointmentsDataTask, lastChangedMoodTask);

            var userInfoResponses = await GetAppointmentsWithAttendeesAsync(appointmentsDataTask.Result.Appointments, cancellationToken);

            return MapResponse(query, appointmentsDataTask.Result.Appointments, lastChangedMoodTask.Result.Items, userInfoResponses);
        }

        private Task<AppointmentDomainServiceDto.AppointmentListResponseDto> GetAppointments(DateTime? appointmentsLastUpdate, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserProvider.GetId();
            if (appointmentsLastUpdate.HasValue)
            {
                var requestToAppointments = new AppointmentDomainServiceDto.ChangedAppointmentListRequestDto(currentUserId, appointmentsLastUpdate.Value);
                return _appointmentService.GetChangedAppointmentList(requestToAppointments, cancellationToken);
            }

            var request = new AppointmentDomainServiceDto.AppointmentListRequestDto { AttendeeId = currentUserId };
            return _appointmentService.GetAppointmentList(request, cancellationToken);
        }

        private Task<PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>>> GetLastChangedMood(
            GetLastChangedDataQuery query, CancellationToken cancellationToken)
        {
            var requestToMoodMeasurement = new HealthMeasurementDomain.GetMeasurementListRequestDto
            {
                // we need only one last mood here
                Paging = PagingRequestDto.FirstItem,
                Filter = new HealthMeasurementDomain.MeasurementListFilterRequestDto
                {
                    PatientId = _currentUserProvider.GetId(),
                    DateRange = new Range<DateTime?> { From = query.MoodLastUpdate }
                }
            };

            return _moodMeasurementService.GetMoodList(requestToMoodMeasurement, cancellationToken);
        }

        private Task<UserInfoResponseDto[]> GetAppointmentsWithAttendeesAsync(IReadOnlyCollection<AppointmentDomainServiceDto.AppointmentResponseDto> appointments, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserProvider.GetId();
            var attendeeIds = appointments.SelectMany(a => a.Attendees)
                .Distinct().Except(new[] { currentUserId }).ToArray(); // we need attendees without current user

            return GetAttendeesByIdsAsync(attendeeIds, cancellationToken);
        }

        private Task<UserInfoResponseDto[]> GetAttendeesByIdsAsync(IEnumerable<Guid> attendeeIds, CancellationToken cancellationToken)
        {
            // TODO: JD-554 заменить на вызов массива Id
            var userInfoTasks = attendeeIds.Select(attendeeId => _usersQueryService.GetUserInfoAsync(attendeeId, cancellationToken));

            return Task.WhenAll(userInfoTasks);
        }

        private LastChangedDataResponseDto MapResponse(GetLastChangedDataQuery query, IEnumerable<AppointmentDomainServiceDto.AppointmentResponseDto> appointments,
            IEnumerable<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>> lastChangedMoodList, IEnumerable<UserInfoResponseDto> userInfoResponses)
        {
            var attendeesList = _mapper.Map<AttendeeResponseDto[]>(userInfoResponses);
            var appointmentsWithAttendeesList = MapToAppointmentResponseDto(appointments, attendeesList);

            var moodResponse = MapMoodResponse(query, lastChangedMoodList);

            return new LastChangedDataResponseDto
            {
                Appointments = appointmentsWithAttendeesList.ToArray(),
                Mood = moodResponse
            };
        }

        private IEnumerable<AppointmentResponseDto> MapToAppointmentResponseDto(IEnumerable<AppointmentDomainServiceDto.AppointmentResponseDto> appointments, IEnumerable<AttendeeResponseDto> attendeesList)
        {
            AttendeeResponseDto[] GetAttendeeResponseByAppointment(AppointmentDomainServiceDto.AppointmentResponseDto _) =>
                _.Attendees.Join(attendeesList, attendeeId => attendeeId, attendee => attendee.Id, (_, attendee) => attendee).ToArray();

            var appointmentAttendeeList = appointments.ToDictionary(_ => _.Id, GetAttendeeResponseByAppointment);

            var appointmentsResponse = _mapper.Map<IReadOnlyCollection<AppointmentResponseDto>>(appointments);
            appointmentsResponse = _mapper.Map(appointmentAttendeeList, appointmentsResponse);
            return appointmentsResponse;
        }

        private MoodResponseDto? MapMoodResponse(GetLastChangedDataQuery query, IEnumerable<HealthMeasurementDomain.MeasurementResponseDto<MoodMeasurementDto>> lastChangedMoodList)
        {
            var lastChangedMood = lastChangedMoodList.FirstOrDefault();
            if (lastChangedMood is null || lastChangedMood.ClientDate == query.MoodLastUpdate)
            {
                return null;
            }

            return _mapper.Map<MoodResponseDto>(lastChangedMood);

        }
    }
}
