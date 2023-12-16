using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Queries;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConferencesController : ServiceBaseController, IVideoConferenceCommandService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateConferenceCommand, Guid> _createConferenceCommandHandler;
        private readonly IQueryHandler<GetConnectionInfoQuery, ConnectionInfoResponseDto> _getConnectionInfoQueryHandler;

        public ConferencesController(IMapper mapper, ICommandHandler<CreateConferenceCommand, Guid> createConferenceCommandHandle, IQueryHandler<GetConnectionInfoQuery,
            ConnectionInfoResponseDto> getConnectionInfoQueryHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _createConferenceCommandHandler = createConferenceCommandHandle ?? throw new ArgumentNullException(nameof(createConferenceCommandHandle));
            _getConnectionInfoQueryHandler = getConnectionInfoQueryHandler ?? throw new ArgumentNullException(nameof(getConnectionInfoQueryHandler));
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid> CreateAsync([FromBody] CreateConferenceRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateConferenceCommand>(request);
            var result = await _createConferenceCommandHandler.HandleAsync(command, cancellationToken);
            return result;
        }

        [HttpGet("{appointmentId}")]
        public async Task<ConnectionInfoResponseDto> GetConnectionInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default)
        {
            var result = await _getConnectionInfoQueryHandler.HandleAsync(new GetConnectionInfoQuery(appointmentId), cancellationToken);
            return result;
        }
    }
}
