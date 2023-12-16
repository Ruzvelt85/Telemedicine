using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.WebClientBffService.WebAPI.Services;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.PulseRate
{
    public class GetPulseRateMeasurementListQueryHandler : IQueryHandler<GetPulseRateMeasurementListQuery, MeasurementListResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPulseRateMeasurementQueryService _service;
        private readonly IHealthMeasurementAccessProvider _healthMeasurementAccessProvider;

        public GetPulseRateMeasurementListQueryHandler(IMapper mapper, IPulseRateMeasurementQueryService service,
            IHealthMeasurementAccessProvider healthMeasurementAccessProvider)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _healthMeasurementAccessProvider = healthMeasurementAccessProvider ?? throw new ArgumentNullException(nameof(healthMeasurementAccessProvider));
        }

        public async Task<MeasurementListResponse> HandleAsync(GetPulseRateMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            // TODO: Yasnobulkov JD-1404 Extract date shift to attribute
            Range<DateTime?>? trimmedDateRange = await _healthMeasurementAccessProvider.TrimDateFilterAccordingToLastAppointmentAsync
                (query.Filter.PatientId, query.Filter.DateRange, cancellationToken);

            if (trimmedDateRange is null)
            { return MeasurementListResponse.Empty; }

            GetMeasurementListRequestDto requestDto = MapWithTrimmedDate(query, trimmedDateRange);

            var saturationListResponse = await _service.GetPulseRateList(requestDto, cancellationToken);

            return _mapper.Map<MeasurementListResponse>(saturationListResponse);
        }

        private GetMeasurementListRequestDto MapWithTrimmedDate(GetPulseRateMeasurementListQuery query, Range<DateTime?> dateRange)
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
