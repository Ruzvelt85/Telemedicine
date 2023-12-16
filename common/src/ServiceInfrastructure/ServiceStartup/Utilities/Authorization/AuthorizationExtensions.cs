using Microsoft.AspNetCore.Builder;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Registers <see cref="UserIdentityInfoEnricherMiddleware"/> into the request pipeline if required
        /// </summary>
        /// <param name="app">The <see cref="Microsoft.AspNetCore.Builder.IApplicationBuilder"/> to add the middleware to</param>
        /// <param name="isUseAuthorization">if 'true' registers <see cref="UserIdentityInfoEnricherMiddleware"/> into the request pipeline </param>
        /// <returns></returns>
        public static IApplicationBuilder UseIdentityInfoEnricher(this IApplicationBuilder app, bool isUseAuthorization)
        {
            if (isUseAuthorization)
            {
                app.UseMiddleware<UserIdentityInfoEnricherMiddleware>();
            }

            return app;
        }
    }
}
