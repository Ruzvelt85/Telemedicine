using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries
{
    public class GetPulseRateListQueryHandler : IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IPulseRateMeasurementReadRepository _pulseRateMeasurementReadRepository;

        public GetPulseRateListQueryHandler(IMapper mapper, IPulseRateMeasurementReadRepository pulseRateMeasurementReadRepository)
        {
            _mapper = mapper;
            _pulseRateMeasurementReadRepository = pulseRateMeasurementReadRepository;
        }

        public async Task<PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>> HandleAsync(GetMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            var specification = GetSpecification(query);

            var pulseRatesList = await _pulseRateMeasurementReadRepository
                .Find(specification)
                .OrderByDescending(m => m.ClientDate)
                .Paginate(query.Paging.Skip, query.Paging.Take)
                .ToListAsync(cancellationToken);

            var totalMeasurements = await _pulseRateMeasurementReadRepository.CountAsync(specification, cancellationToken);
            var response = _mapper.Map<PagedListResponseDto<MeasurementResponseDto<PulseRateMeasurementDto>>>(pulseRatesList)
                with
            { Paging = new PagingResponseDto(totalMeasurements) };

            return response;
        }

        private static Specification<PulseRateMeasurement> GetSpecification(GetMeasurementListQuery query)
        {
            var patientIdSpecification = new PatientIdSpec(query.Filter.PatientId).As<PulseRateMeasurement>();
            var dateSpecification = new ClientDateRangeSpec(query.Filter.DateRange.From, query.Filter.DateRange.To).As<PulseRateMeasurement>();

            return patientIdSpecification & dateSpecification;
        }
    }
}
