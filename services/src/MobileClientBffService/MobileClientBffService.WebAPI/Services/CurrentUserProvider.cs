using System;
using System.Linq;
using System.Security.Claims;
using Telemedicine.Common.Business.BusinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Services
{
    /// <summary>
    /// It gets the current user from claims that we got from a JWT 
    /// </summary>
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtBearerClaimsSettings _jwtBearerClaimsSettings;

        public CurrentUserProvider(IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<JwtBearerClaimsSettings> jwtBearerClaimsSettingsOptionsSnapshot)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtBearerClaimsSettings = jwtBearerClaimsSettingsOptionsSnapshot.Value;
        }

        public Guid GetId()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            Claim? userIdClaim = user?.Claims.FirstOrDefault(c => c.Type == _jwtBearerClaimsSettings.UserId);

            if (userIdClaim is null)
            { throw new UserClaimException($"Couldn't find the claim type {_jwtBearerClaimsSettings.UserId}", UserClaimException.ErrorType.ClaimDoesnotExist, _jwtBearerClaimsSettings.UserId); }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            { throw new UserClaimException($"The value of the claim {userIdClaim.Type} can't be parsed into GUID", UserClaimException.ErrorType.ClaimValueIsInvalid, userIdClaim.Type, userIdClaim.Value); }

            return userId;
        }
    }
}
