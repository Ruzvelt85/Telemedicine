using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCentersController : DomainServiceBaseController, IHealthCentersCommandService
    {
        private readonly IMapper _mapper;
        private readonly ICommandHandler<CreateOrUpdateHealthCenterCommand, Guid?> _createOrUpdateHealthCenterHandler;

        public HealthCentersController(IMapper mapper,
            ICommandHandler<CreateOrUpdateHealthCenterCommand, Guid?> createOrUpdateHealthCenterHandler)
        {
            _mapper = mapper;
            _createOrUpdateHealthCenterHandler = createOrUpdateHealthCenterHandler;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid?> CreateOrUpdateAsync([FromBody] CreateOrUpdateHealthCenterRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateOrUpdateHealthCenterCommand>(request);
            var result = await _createOrUpdateHealthCenterHandler.HandleAsync(command, cancellationToken);
            return result;
        }
    }
}
