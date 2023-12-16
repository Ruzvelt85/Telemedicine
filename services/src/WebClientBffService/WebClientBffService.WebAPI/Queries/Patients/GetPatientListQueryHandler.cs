using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using AppointmentServiceDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using PatientsQueryServiceDto = Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients
{
    public class GetPatientListQueryHandler : IQueryHandler<GetPatientListQuery, PatientListResponseDto>
    {
        private readonly IPatientsQueryService _paceStructureServiceApi;
        private readonly IWebClientBffQueryService _appointmentServiceApi;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IMapper _mapper;

        public GetPatientListQueryHandler(IMapper mapper, ICurrentUserProvider currentUserProvider,
            IPatientsQueryService paceStructureServiceApi, IWebClientBffQueryService appointmentServiceApi)
        {
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _paceStructureServiceApi = paceStructureServiceApi;
            _appointmentServiceApi = appointmentServiceApi;
        }

        public async Task<PatientListResponseDto> HandleAsync(GetPatientListQuery query, CancellationToken cancellationToken = default)
        {
            var patientsRequest = _mapper.Map<PatientsQueryServiceDto.PatientListRequestDto>(query);
            patientsRequest = patientsRequest with
            {
                Filter = patientsRequest.Filter != null
                    ? patientsRequest.Filter with { DoctorId = _currentUserProvider.GetId() }
                    : new PatientsQueryServiceDto.PatientListFilterRequestDto { DoctorId = _currentUserProvider.GetId() }
            };
            var patientsResponse = await _paceStructureServiceApi.GetPatientListAsync(patientsRequest, cancellationToken);

            var items = new PatientResponseDto[(patientsResponse.Items.Count)];

            // TODO: JD-554 заменить на вызов массива Id
            await Task.WhenAll(patientsResponse.Items.Select(async (patient, i) =>
            {
                var appointmentsResponse = await _appointmentServiceApi.GetNearestAppointmentsByAttendeeIdAsync(patient.Id, cancellationToken);

                var mappedItem = MapToPatientResponseDto(patient, appointmentsResponse);

                items[i] = mappedItem;
            }));

            return new PatientListResponseDto
            {
                Paging = patientsResponse.Paging,
                Items = items
            };
        }

        /// <summary>
        /// Make mapping of two responses to the only DTO
        /// </summary>
        private PatientResponseDto MapToPatientResponseDto(PatientsQueryServiceDto.PatientResponseDto patient, AppointmentServiceDto.NearestAppointmentsResponseDto appointments)
        {
            var result = _mapper.Map<PatientResponseDto>(patient);

            return _mapper.Map(appointments, result);
        }
    }
}
