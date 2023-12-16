using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries
{
    public class GetSaturationMeasurementListQueryHandler : IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ISaturationMeasurementReadRepository _readRepository;

        public GetSaturationMeasurementListQueryHandler(IMapper mapper, ISaturationMeasurementReadRepository readRepository)
        {
            _mapper = mapper;
            _readRepository = readRepository;
        }

        /// <inheritdoc />
        public async Task<PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>> HandleAsync(GetMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            var specification = GetSpecification(query);

            var saturationMeasurements = await _readRepository
                .Find(specification)
                .OrderByDescending(m => m.ClientDate)
                .Paginate(query.Paging.Skip, query.Paging.Take)
                .ToListAsync(cancellationToken);

            var totalSaturationMeasurements = await _readRepository.CountAsync(specification, cancellationToken);
            var response = _mapper.Map<PagedListResponseDto<MeasurementResponseDto<SaturationMeasurementDto>>>(saturationMeasurements)
                with
            { Paging = new PagingResponseDto(totalSaturationMeasurements) };
            return response;
        }

        private static Specification<SaturationMeasurement> GetSpecification(GetMeasurementListQuery query)
        {
            var patientIdSpecification = new PatientIdSpec(query.Filter.PatientId).As<SaturationMeasurement>();
            var dateSpecification = new ClientDateRangeSpec(query.Filter.DateRange.From, query.Filter.DateRange.To).As<SaturationMeasurement>();

            return patientIdSpecification & dateSpecification;
        }
    }
}
