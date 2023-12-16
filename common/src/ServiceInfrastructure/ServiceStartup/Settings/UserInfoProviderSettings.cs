using JetBrains.Annotations;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings
{
    internal record UserInfoProviderSettings
    {
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// URL to endpoint that contains user info
        /// </summary>
        public string? Url { get; init; }

        /// <summary>
        /// Determines which user info provider will be used to fetch the additional user info
        /// </summary>
        public UserInfoProviderType UserInfoProviderType { get; init; } = UserInfoProviderType.OpenIdConnect;
    }

    [PublicAPI]
    internal enum UserInfoProviderType
    {
        OpenIdConnect = 0,
        CognitoApi = 1
    }
}
