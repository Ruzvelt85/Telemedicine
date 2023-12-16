using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    /// <summary>
    /// Enriches user identity with additional claims fetched from via <see cref="IUserInfoProvider"/>
    /// </summary>
    internal class UserIdentityInfoEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Log.ForContext<UserIdentityInfoEnricherMiddleware>();

        public UserIdentityInfoEnricherMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        [UsedImplicitly]
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                var settingsOptionsSnapshot = httpContext.RequestServices.GetService(typeof(IOptionsSnapshot<UserInfoProviderSettings>)) as IOptionsSnapshot<UserInfoProviderSettings>;
                var userInfoProvider = httpContext.RequestServices.GetService(typeof(IUserInfoProvider)) as IUserInfoProvider;
                if (settingsOptionsSnapshot is null || userInfoProvider is null)
                {
                    throw new Exception($"The object of type {nameof(IUserInfoProvider)} or {nameof(IOptionsSnapshot<UserInfoProviderSettings>)} is not registered in the DI container, we can't enrich user identity claims");
                }

                await InvokeAsyncImpl(httpContext, settingsOptionsSnapshot.Value, userInfoProvider);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected exception occurred while enriching identity claims with additional info");
            }
            finally
            {
                await _next(httpContext);
            }
        }

        private async Task InvokeAsyncImpl(HttpContext httpContext, UserInfoProviderSettings settings, IUserInfoProvider userInfoProvider)
        {
            if (settings.IsEnabled)
            {
                await EnrichIdentityWithUserInfo(httpContext, userInfoProvider);
            }
            else
            {
                _logger.Debug("{UserIdentityInfoEnricherMiddleware} is disabled", nameof(UserIdentityInfoEnricherMiddleware));
            }
        }

        private async Task EnrichIdentityWithUserInfo(HttpContext httpContext, IUserInfoProvider userInfoProvider)
        {
            string token = await GetToken(httpContext);
            Dictionary<string, string>? userInfoClaimsDict = await userInfoProvider.GetUserInfoAsync(token);
            if (userInfoClaimsDict is not null)
            {
                AddClaimsToIdentity(httpContext, userInfoClaimsDict);
            }
        }

        private async Task<string> GetToken(HttpContext httpContext)
        {
            _logger.Verbose("We are going to get token from the httpContext");

            string? token = await httpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken); //if we get an invalid token the method will return null
            if (token is null)
            {
                throw new Exception("The access token is not present in the httpContext, so we can't enrich the user identity with additional claims");
            }

            return token;
        }

        private void AddClaimsToIdentity(HttpContext context, Dictionary<string, string> userInfoClaimsDict)
        {
            _logger.Verbose("We are going to add claims to the Identity");

            ClaimsPrincipal principal = context.User;
            ClaimsIdentity? identity = principal.Identity as ClaimsIdentity;
            if (identity is null)
            {
                Debug.Assert(false, "We couldn't get ClaimsIdentity from current user");
                throw new Exception("We couldn't get ClaimsIdentity from current user");
            }

            foreach (var claim in userInfoClaimsDict)
            {
                if (!principal.HasClaim(c => c.Type == claim.Key))
                {
                    identity.AddClaim(new Claim(claim.Key, claim.Value));
                }
            }

            _logger.Debug("We successfully added the following claims into the identity: {@Claims}", userInfoClaimsDict);
        }
    }
}
