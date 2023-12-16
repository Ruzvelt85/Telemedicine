using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientById;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients;
using Microsoft.AspNetCore.Mvc;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Controllers
{
    [Route("api/users/[controller]")]
    [Route("api/[controller]")] // TODO: Sapegin, JD-1377 - Remove old path
    public class PatientsController : BffServiceBaseController, IPatientQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetPatientListQuery, PatientListResponseDto> _getPatientListQueryHandler;
        private readonly IQueryHandler<GetPatientByIdQuery, PatientByIdResponseDto> _getPatientByIdQueryHandler;
        private readonly IQueryHandler<GetAppointmentListByPatientIdQuery, AppointmentListByPatientIdResponseDto> _getAppointmentListByPatientIdQueryHandler;

        public PatientsController(IMapper mapper,
            IQueryHandler<GetPatientListQuery, PatientListResponseDto> getPatientListQueryHandler,
            IQueryHandler<GetPatientByIdQuery, PatientByIdResponseDto> getPatientByIdQueryHandler,
            IQueryHandler<GetAppointmentListByPatientIdQuery, AppointmentListByPatientIdResponseDto> getAppointmentListByPatientIdQueryHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getPatientListQueryHandler = getPatientListQueryHandler ?? throw new ArgumentNullException(nameof(getPatientListQueryHandler));
            _getPatientByIdQueryHandler = getPatientByIdQueryHandler ?? throw new ArgumentNullException(nameof(getPatientByIdQueryHandler));
            _getAppointmentListByPatientIdQueryHandler = getAppointmentListByPatientIdQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentListByPatientIdQueryHandler));
        }

        [HttpGet]
        public async Task<PatientListResponseDto> GetPatientListAsync([FromQuery] PatientListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetPatientListQuery>(request);
            PatientListResponseDto patientList = await _getPatientListQueryHandler.HandleAsync(query, cancellationToken);
            return patientList;
        }

        [HttpGet("{id}")]
        public async Task<PatientByIdResponseDto> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _getPatientByIdQueryHandler.HandleAsync(new GetPatientByIdQuery(id), cancellationToken);
            return result;
        }

        [HttpGet("{id}/appointments")]
        public async Task<AppointmentListByPatientIdResponseDto> GetAppointmentsByPatientIdAsync(Guid id, [FromQuery] AppointmentListByPatientIdRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetAppointmentListByPatientIdQuery>(request) with { PatientId = id };
            var result = await _getAppointmentListByPatientIdQueryHandler.HandleAsync(query, cancellationToken);
            return result;
        }
    }
}
