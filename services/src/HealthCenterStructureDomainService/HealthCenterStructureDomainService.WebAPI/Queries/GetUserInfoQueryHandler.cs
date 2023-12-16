using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public class GetUserInfoQueryHandler : IQueryHandler<GetUserInfoQuery, User>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetUserInfoQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository ?? throw new ArgumentNullException(nameof(userReadRepository));
        }

        /// <inheritdoc />
        public async Task<User> HandleAsync(GetUserInfoQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _userReadRepository.GetByIdAsync(query.UserId, cancellationToken);

            if (user == null)
            { throw new EntityNotFoundByIdException(typeof(User), query.UserId); }

            return user;
        }
    }
}
