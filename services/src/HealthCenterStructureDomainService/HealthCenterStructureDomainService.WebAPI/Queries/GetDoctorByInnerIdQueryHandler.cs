using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public class GetDoctorByInnerIdQueryHandler : IQueryHandler<GetDoctorByInnerIdQuery, DoctorByInnerIdResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<Doctor> _doctorReadRepository;

        public GetDoctorByInnerIdQueryHandler(IMapper mapper, IReadRepository<Doctor> doctorReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _doctorReadRepository = doctorReadRepository ?? throw new ArgumentNullException(nameof(doctorReadRepository));
        }

        /// <inheritdoc />
        public async Task<DoctorByInnerIdResponseDto> HandleAsync(GetDoctorByInnerIdQuery query, CancellationToken cancellationToken = default)
        {
            var findByInnerIdSpec = new ByInnerIdSpecification<Doctor>(query.InnerId);

            var doctor = await _doctorReadRepository
                    .Find(findByInnerIdSpec)
                    .Include(_ => _.HealthCenters)
                    .FirstOrDefaultAsync(cancellationToken);

            if (doctor is null)
            { throw new EntityNotFoundByInnerIdException(typeof(Doctor), query.InnerId); }

            var doctorResponse = _mapper.Map<DoctorByInnerIdResponseDto>(doctor);
            return doctorResponse;
        }
    }
}
