using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientById;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientByIdResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IPatientReadRepository _patientReadRepository;

        public GetPatientByIdQueryHandler(IMapper mapper, IPatientReadRepository patientReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _patientReadRepository = patientReadRepository ?? throw new ArgumentNullException(nameof(patientReadRepository));
        }

        /// <inheritdoc />
        public async Task<PatientByIdResponseDto> HandleAsync(GetPatientByIdQuery query, CancellationToken cancellationToken = default)
        {
            var patient = await _patientReadRepository
                .Find(_ => _.Id == query.Id)
                .ProjectTo<PatientByIdResponseDto>(_mapper.ConfigurationProvider) // No need for explicit Include for the navigation properties
                .FirstOrDefaultAsync(cancellationToken);

            if (patient == null)
            { throw new EntityNotFoundByIdException(typeof(Patient), query.Id); }

            return patient;
        }
    }
}
