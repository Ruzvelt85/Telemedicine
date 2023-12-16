using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.BloodPressure;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Mood;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.PulseRate;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Saturation;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements
{
    public class GetHealthMeasurementListQueryHandler : IQueryHandler<GetHealthMeasurementListQuery, MeasurementListResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        // JD-812 Use Func because can`t cast Query objects to abstract IGetMeasurementListQuery type in IQueryHandler
        private readonly IDictionary<MeasurementType, Func<GetMeasurementListQuery, Task<MeasurementListResponse>>> _queryHandlerDictionary;

        public GetHealthMeasurementListQueryHandler(IMapper mapper,
            IQueryHandler<GetBloodPressureMeasurementListQuery, MeasurementListResponse> bloodPressureQueryHandler,
            IQueryHandler<GetSaturationMeasurementListQuery, MeasurementListResponse> saturationQueryHandler,
            IQueryHandler<GetPulseRateMeasurementListQuery, MeasurementListResponse> pulseRateQueryHandler,
            IQueryHandler<GetMoodMeasurementListQuery, MeasurementListResponse> moodQueryHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = Log.ForContext<GetHealthMeasurementListQuery>();
            _queryHandlerDictionary = new Dictionary<MeasurementType, Func<GetMeasurementListQuery, Task<MeasurementListResponse>>>
            {
                { MeasurementType.BloodPressure, (query) => bloodPressureQueryHandler.HandleAsync(_mapper.Map<GetBloodPressureMeasurementListQuery>(query)) },
                { MeasurementType.Saturation, (query) => saturationQueryHandler.HandleAsync(_mapper.Map<GetSaturationMeasurementListQuery>(query)) },
                { MeasurementType.PulseRate, (query) => pulseRateQueryHandler.HandleAsync(_mapper.Map<GetPulseRateMeasurementListQuery>(query)) },
                { MeasurementType.Mood, (query) => moodQueryHandler.HandleAsync(_mapper.Map<GetMoodMeasurementListQuery>(query)) },
            };
        }

        public Task<MeasurementListResponseDto> HandleAsync(GetHealthMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            var requestsToDomainServices = GetDomainRequests(query).ToArray();
            var requestsResults = GetDomainResults(requestsToDomainServices, cancellationToken);

            IReadOnlyCollection<IHasClientDate> pagedMeasurements = requestsResults
                .SelectMany(r => r.Items)
                .OrderByDescending(m => m.ClientDate)
                .Skip(query.Paging.Skip)
                .Take(query.Paging.Take)
                .ToList();

            var pagingTotal = requestsResults.Sum(r => r.Paging.Total);

            var response = _mapper.Map<MeasurementListResponseDto>(pagedMeasurements)
                with
            {
                Paging = new PagingResponseDto(pagingTotal)
            };

            return Task.FromResult(response);
        }

        public IEnumerable<Task<MeasurementListResponse>> GetDomainRequests(GetHealthMeasurementListQuery query)
        {
            var internalQuery = _mapper.Map<GetMeasurementListQuery>(query);
            foreach (var enumValue in Enum.GetValues<MeasurementType>())
            {
                if (enumValue == MeasurementType.All || !query.Filter.MeasurementType.HasFlag(enumValue))
                {
                    continue;
                }

                if (!_queryHandlerDictionary.TryGetValue(enumValue, out var queryHandler))
                {
                    _logger.Warning("QueryHandler for measurement type {0} not added to {1} in {2}",
                        enumValue.ToString(), nameof(_queryHandlerDictionary), nameof(GetHealthMeasurementListQueryHandler));
                    continue;
                }

                yield return queryHandler.Invoke(internalQuery);
            }
        }

        private IReadOnlyCollection<MeasurementListResponse> GetDomainResults(
            IReadOnlyCollection<Task<MeasurementListResponse>> requestsToDomainServices, CancellationToken cancellationToken)
        {
            Task.WhenAll(requestsToDomainServices).Wait(cancellationToken);

            var resultsAndExceptions = requestsToDomainServices.ToLookup(el => el.IsFaulted);

            var requestsResults = resultsAndExceptions[false]
                .Select(el => el.Result)
                .ToArray();

            var requestsExceptions = resultsAndExceptions[true]
                .Select(el => el.Exception);

            foreach (var requestException in requestsExceptions)
            {
                _logger.Warning(requestException, "Exception was thrown while sending a request to the HealthMeasurementDomainService");
            }

            return requestsResults;
        }
    }
}
