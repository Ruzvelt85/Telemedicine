using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ServiceBaseController, IUsersQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetUserInfoQuery, User> _getUserInfoQueryHandler;

        public UsersController(IMapper mapper, IQueryHandler<GetUserInfoQuery, User> getUserInfoQueryHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getUserInfoQueryHandler = getUserInfoQueryHandler ?? throw new ArgumentNullException(nameof(getUserInfoQueryHandler));
        }

        [HttpGet("{userId}")]
        public async Task<UserInfoResponseDto> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _getUserInfoQueryHandler.HandleAsync(new GetUserInfoQuery(userId), cancellationToken);
            var userResponseDto = _mapper.Map<UserInfoResponseDto>(user);
            return userResponseDto;
        }

        [HttpGet("{userId}/details")]
        public async Task<UserInfoDetailsResponseDto> GetUserInfoDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _getUserInfoQueryHandler.HandleAsync(new GetUserInfoQuery(userId), cancellationToken);
            var userResponseDto = _mapper.Map<UserInfoDetailsResponseDto>(user);
            return userResponseDto;
        }
    }
}
