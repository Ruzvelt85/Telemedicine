using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries
{
    public class GetMoodListQueryHandler : IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IMoodMeasurementReadRepository _moodMeasurementReadRepository;

        public GetMoodListQueryHandler(IMapper mapper, IMoodMeasurementReadRepository moodMeasurementReadRepository)
        {
            _mapper = mapper;
            _moodMeasurementReadRepository = moodMeasurementReadRepository;
        }

        public async Task<PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>> HandleAsync(
            GetMeasurementListQuery query, CancellationToken cancellationToken = default)
        {
            var specification = GetSpecification(query);

            var moodList = await _moodMeasurementReadRepository
                .Find(specification)
                .OrderByDescending(m => m.ClientDate)
                .Paginate(query.Paging.Skip, query.Paging.Take)
                .ToListAsync(cancellationToken);

            var totalMeasurements = await _moodMeasurementReadRepository.CountAsync(specification, cancellationToken);

            var response = _mapper.Map<PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>>(moodList)
                with
            { Paging = new PagingResponseDto(totalMeasurements) };
            return response;
        }

        private static Specification<MoodMeasurement> GetSpecification(GetMeasurementListQuery query)
        {
            var patientIdSpecification = new PatientIdSpec(query.Filter.PatientId).As<MoodMeasurement>();
            var dateSpecification = new ClientDateRangeSpec(query.Filter.DateRange.From, query.Filter.DateRange.To).As<MoodMeasurement>();

            return patientIdSpecification & dateSpecification;
        }
    }
}
