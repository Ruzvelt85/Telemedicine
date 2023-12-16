using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries
{
    public class GetBloodPressureListQueryHandler : IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IBloodPressureMeasurementReadRepository _bloodPressureMeasurementReadRepository;

        public GetBloodPressureListQueryHandler(IMapper mapper, IBloodPressureMeasurementReadRepository bloodPressureMeasurementReadRepository)
        {
            _mapper = mapper;
            _bloodPressureMeasurementReadRepository = bloodPressureMeasurementReadRepository;
        }

        public async Task<PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>> HandleAsync(GetMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            var specification = GetSpecification(query);

            var bloodPressuresList = await _bloodPressureMeasurementReadRepository
                .Find(specification)
                .OrderByDescending(m => m.ClientDate)
                .Paginate(query.Paging.Skip, query.Paging.Take)
                .ToListAsync(cancellationToken);

            var totalMeasurements = await _bloodPressureMeasurementReadRepository.CountAsync(specification, cancellationToken);
            var response = _mapper.Map<PagedListResponseDto<MeasurementResponseDto<BloodPressureMeasurementDto>>>(bloodPressuresList)
                with
            { Paging = new PagingResponseDto(totalMeasurements) };

            return response;
        }

        private static Specification<BloodPressureMeasurement> GetSpecification(GetMeasurementListQuery query)
        {
            var patientIdSpecification = new PatientIdSpec(query.Filter.PatientId).As<BloodPressureMeasurement>();
            var dateSpecification = new ClientDateRangeSpec(query.Filter.DateRange.From, query.Filter.DateRange.To).As<BloodPressureMeasurement>();

            return patientIdSpecification & dateSpecification;
        }
    }
}
