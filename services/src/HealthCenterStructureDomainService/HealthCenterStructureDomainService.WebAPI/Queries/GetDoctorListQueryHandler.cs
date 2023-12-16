using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications.Doctor;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public class GetDoctorListQueryHandler : IQueryHandler<GetDoctorListQuery, PagedListResponseDto<DoctorResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IDoctorReadRepository _doctorReadRepository;

        public GetDoctorListQueryHandler(IMapper mapper,
            IDoctorReadRepository doctorReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _doctorReadRepository = doctorReadRepository ?? throw new ArgumentNullException(nameof(doctorReadRepository));
        }

        /// <inheritdoc />
        public async Task<PagedListResponseDto<DoctorResponseDto>> HandleAsync(GetDoctorListQuery query, CancellationToken cancellationToken = default)
        {
            Specification<Doctor> specificationToFilterDoctors = GetSpecificationToFilterDoctors(query.Filter);

            return await _doctorReadRepository
                .Find(specificationToFilterDoctors)
                .Include(me => me.HealthCenters)
                .OrderBy(p => p.FirstName, query.FirstNameSortingType)
                .ToPagedResponseAsync<Doctor, DoctorResponseDto>(_mapper, query.Paging, cancellationToken);
        }

        /// <summary>
        /// Gets specification for filtering doctors
        /// </summary>
        private Specification<Doctor> GetSpecificationToFilterDoctors(DoctorListFilterRequestDto queryFilter)
        {
            Specification<Doctor> doctorsByNameSpec = string.IsNullOrWhiteSpace(queryFilter.Name)
                ? new TrueSpecification<Doctor>()
                : new ByPersonNameSpecification<Doctor>(queryFilter.Name);

            Specification<Doctor> assignedToHealthCenterSpec = queryFilter.HealthCenterIds?.Length > 0
                ? new DoctorByHealthCenterSpecification(queryFilter.HealthCenterIds.Distinct().ToArray())
                : new TrueSpecification<Doctor>();

            return assignedToHealthCenterSpec & doctorsByNameSpec;
        }
    }
}
