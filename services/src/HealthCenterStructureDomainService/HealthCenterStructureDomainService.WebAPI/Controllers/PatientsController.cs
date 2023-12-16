using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientById;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : DomainServiceBaseController, IPatientsQueryService, IPatientsCommandService
    {
        private readonly IMapper _mapper;

        private readonly IQueryHandler<GetPatientsQuery, PatientListResponseDto> _getPatientsQueryHandler;
        private readonly IQueryHandler<GetPatientByIdQuery, PatientByIdResponseDto> _getPatientByIdQueryHandler;
        private readonly ICommandHandler<CreateOrUpdatePatientCommand, Guid?> _createOrUpdatePatientHandler;


        public PatientsController(IMapper mapper,
            IQueryHandler<GetPatientsQuery, PatientListResponseDto> getPatientsQueryHandler,
            IQueryHandler<GetPatientByIdQuery, PatientByIdResponseDto> getPatientByIdQueryHandler,
            ICommandHandler<CreateOrUpdatePatientCommand, Guid?> createOrUpdatePatientHandler)
        {
            _mapper = mapper;
            _getPatientsQueryHandler = getPatientsQueryHandler;
            _getPatientByIdQueryHandler = getPatientByIdQueryHandler;
            _createOrUpdatePatientHandler = createOrUpdatePatientHandler;
        }

        [HttpGet]
        public async Task<PatientListResponseDto> GetPatientListAsync([FromQuery] PatientListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetPatientsQuery>(request);
            var patients = await _getPatientsQueryHandler.HandleAsync(query, cancellationToken);
            return _mapper.Map<PatientListResponseDto>(patients);
        }

        [HttpGet("{id}")]
        public async Task<PatientByIdResponseDto> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _getPatientByIdQueryHandler.HandleAsync(new GetPatientByIdQuery(id), cancellationToken);
            return response;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid?> CreateOrUpdateAsync([FromBody] CreateOrUpdatePatientRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateOrUpdatePatientCommand>(request);
            var result = await _createOrUpdatePatientHandler.HandleAsync(command, cancellationToken);
            return result;
        }
    }
}
