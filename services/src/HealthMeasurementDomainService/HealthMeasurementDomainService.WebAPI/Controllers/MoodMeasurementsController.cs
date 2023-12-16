using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoodMeasurementsController : DomainServiceBaseController, IMoodMeasurementCommandService, IMoodMeasurementQueryService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateMeasurementCommand<MoodMeasurementDto>, Guid> _createMoodMeasurementCommandHandler;
        private readonly IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>> _getMoodListQueryHandler;

        public MoodMeasurementsController(IMapper mapper,
            ICommandHandler<CreateMeasurementCommand<MoodMeasurementDto>, Guid> createMoodMeasurementCommandHandler,
            IQueryHandler<GetMeasurementListQuery, PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>> getMoodListQueryHandler)
        {
            _mapper = mapper;
            _createMoodMeasurementCommandHandler = createMoodMeasurementCommandHandler;
            _getMoodListQueryHandler = getMoodListQueryHandler;
        }

        [HttpGet]
        public async Task<PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>> GetMoodList(
            [FromQuery] GetMeasurementListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetMeasurementListQuery>(request);
            var result = await _getMoodListQueryHandler.HandleAsync(query, cancellationToken);
            return result;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid> CreateAsync([FromBody] CreateMeasurementRequestDto<MoodMeasurementDto> request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateMeasurementCommand<MoodMeasurementDto>>(request);
            var result = await _createMoodMeasurementCommandHandler.HandleAsync(command, cancellationToken);
            return result;
        }
    }
}
