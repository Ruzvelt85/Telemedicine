using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientById;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients
{
    public class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientByIdResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IPatientsQueryService _patientsQueryService;

        public GetPatientByIdQueryHandler(IMapper mapper, IPatientsQueryService patientsQueryService)
        {
            _mapper = mapper;
            _patientsQueryService = patientsQueryService;
        }

        /// <inheritdoc />
        public async Task<PatientByIdResponseDto> HandleAsync(GetPatientByIdQuery query, CancellationToken cancellationToken = default)
        {
            // TODO: Savich - JD-817 - Check that current doctor and patient with id (query.Id) from the same health center

            var response = await _patientsQueryService.GetPatientByIdAsync(query.Id, cancellationToken);
            return _mapper.Map<PatientByIdResponseDto>(response);
        }
    }
}
