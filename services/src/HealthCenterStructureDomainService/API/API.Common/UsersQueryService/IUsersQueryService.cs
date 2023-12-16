using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Refit;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService
{
    public interface IUsersQueryService
    {
        /// <exception cref="EntityNotFoundException">Thrown when user was not found</exception>
        [Get("/api/users/{userId}")]
        Task<UserInfoResponseDto> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <exception cref="EntityNotFoundException">Thrown when user was not found</exception>
        [Get("/api/users/{userId}/details")]
        Task<UserInfoDetailsResponseDto> GetUserInfoDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
