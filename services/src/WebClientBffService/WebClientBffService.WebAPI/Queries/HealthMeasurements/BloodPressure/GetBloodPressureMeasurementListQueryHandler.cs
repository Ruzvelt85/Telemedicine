using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.BloodPressure
{
    public class GetBloodPressureMeasurementListQueryHandler : IQueryHandler<GetBloodPressureMeasurementListQuery, MeasurementListResponse>
    {
        private readonly IMapper _mapper;
        private readonly IBloodPressureMeasurementQueryService _service;
        private readonly IHealthMeasurementAccessProvider _healthMeasurementAccessProvider;

        public GetBloodPressureMeasurementListQueryHandler(IMapper mapper, IBloodPressureMeasurementQueryService service,
            IHealthMeasurementAccessProvider healthMeasurementAccessProvider)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _healthMeasurementAccessProvider = healthMeasurementAccessProvider ?? throw new ArgumentNullException(nameof(healthMeasurementAccessProvider));
        }

        public async Task<MeasurementListResponse> HandleAsync(GetBloodPressureMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            // TODO: Yasnobulkov JD-1404 Extract date shift to attribute
            Range<DateTime?>? trimmedDateRange = await _healthMeasurementAccessProvider.TrimDateFilterAccordingToLastAppointmentAsync
                (query.Filter.PatientId, query.Filter.DateRange, cancellationToken);

            if (trimmedDateRange is null)
            { return MeasurementListResponse.Empty; }

            GetMeasurementListRequestDto requestDto = MapWithTrimmedDate(query, trimmedDateRange);
            var bloodPressureListResponse = await _service.GetBloodPressureList(requestDto, cancellationToken);

            return _mapper.Map<MeasurementListResponse>(bloodPressureListResponse);
        }

        private GetMeasurementListRequestDto MapWithTrimmedDate(GetBloodPressureMeasurementListQuery query, Range<DateTime?> dateRange)
        {
            var measurementListRequest = _mapper.Map<GetMeasurementListRequestDto>(query);

            return measurementListRequest
                with
            {
                Filter = measurementListRequest.Filter with { DateRange = dateRange },
                Paging = new PagingRequestDto(measurementListRequest.Paging.Skip + measurementListRequest.Paging.Take)
            };
        }
    }
}
