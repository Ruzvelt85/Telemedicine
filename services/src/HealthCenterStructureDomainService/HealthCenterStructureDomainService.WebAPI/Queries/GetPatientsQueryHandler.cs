using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.InterdisciplinaryTeam;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.HealthCenter;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Patient;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public class GetPatientsQueryHandler : IQueryHandler<GetPatientsQuery, PatientListResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IPatientReadRepository _patientReadRepository;
        private readonly IInterdisciplinaryTeamReadRepository _interdisciplinaryTeamReadRepository;
        private readonly IHealthCenterReadRepository _healthCenterReadRepository;

        public GetPatientsQueryHandler(IMapper mapper,
            IPatientReadRepository patientReadRepository,
            IInterdisciplinaryTeamReadRepository interdisciplinaryTeamReadRepository,
            IHealthCenterReadRepository healthCenterReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _patientReadRepository = patientReadRepository ?? throw new ArgumentNullException(nameof(patientReadRepository));
            _interdisciplinaryTeamReadRepository = interdisciplinaryTeamReadRepository ?? throw new ArgumentNullException(nameof(interdisciplinaryTeamReadRepository));
            _healthCenterReadRepository = healthCenterReadRepository ?? throw new ArgumentNullException(nameof(healthCenterReadRepository));
        }

        /// <inheritdoc />
        public async Task<PatientListResponseDto> HandleAsync(GetPatientsQuery query, CancellationToken cancellationToken = default)
        {
            Specification<Patient> searchPatientSpec = GetSearchPatientSpecificationByFilter(query.Filter);
            Specification<Patient> assignedToTeamOrHealthCenterSpec = await GetAssignedSpecificationByQuery(query, cancellationToken);

            var specification = assignedToTeamOrHealthCenterSpec & searchPatientSpec;

            var patients = await _patientReadRepository
                .Find(specification)
                .OrderBy(p => p.FirstName, query.FirstNameSortingType)
                .Paginate(query.Paging.Skip, query.Paging.Take)
                .ToListAsync(cancellationToken);

            var totalCount = await _patientReadRepository.CountAsync(specification, cancellationToken);

            return _mapper.Map<PatientListResponseDto>(patients) with { Paging = new PagingResponseDto(totalCount) };
        }

        private static Specification<Patient> GetSearchPatientSpecificationByFilter(PatientListFilterRequestDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Name))
            { return new ByPersonNameSpecification<Patient>(filter.Name); }

            if (!string.IsNullOrWhiteSpace(filter.Name))
            { return new ByPersonNameSpecification<Patient>(filter.Name); }

            return new TrueSpecification<Patient>();
        }

        private async Task<Specification<Patient>> GetAssignedSpecificationByQuery(GetPatientsQuery query, CancellationToken cancellationToken)
        {
            switch (query.Filter.HealthCenterStructureFilter)
            {
                case HealthCenterStructureFilterType.InterdisciplinaryTeam:
                    var ids = await GetInterdisciplinaryTeamIdsByDoctorId(query.Filter.DoctorId, cancellationToken);
                    return new PatientAssignedToTeamSpecification(ids);

                case HealthCenterStructureFilterType.HealthCenter:
                    var healthCenterIds = await GetHealthCenterIdsByDoctorId(query.Filter.DoctorId, cancellationToken);
                    return new PatientAssignedToHealthCenterSpecification(healthCenterIds);
                default:
                    throw new ArgumentException("Incorrect HealthCenterStructureFilter");
            }
        }

        internal async Task<IEnumerable<Guid>> GetHealthCenterIdsByDoctorId(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var hasAnyDoctorSpec = new HealthCenterHasAnyDoctorSpecification(doctorId);

            var healthCenterIds = await _healthCenterReadRepository
                .Find(hasAnyDoctorSpec)
                .Select(pc => pc.Id)
                .ToListAsync(cancellationToken);
            return healthCenterIds;
        }

        internal async Task<IEnumerable<Guid>> GetInterdisciplinaryTeamIdsByDoctorId(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var teamHasAnyDoctorSpec = new TeamHasAnyDoctorSpecification(doctorId);

            var teamIds = await _interdisciplinaryTeamReadRepository
                .Find(teamHasAnyDoctorSpec)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);
            return teamIds;
        }
    }
}
