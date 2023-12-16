using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    /// <summary>
    /// Provides additional user info from a remote source.
    /// </summary>
    internal interface IInternalUserInfoProvider
    {
        /// <summary>
        /// Get the user info from a remote source. The user info is taken for a user whose <paramref name="token"/> is provided.
        /// </summary>
        /// <param name="userInfoEndpointUri">uri of the remote source endpoint</param>
        /// <param name="token">token of the user</param>
        /// <returns> null if couldn't get the user info from <paramref name="userInfoEndpointUri"/>, otherwise returns Dictionary where Key is claim type, and value is claim value</returns>
        Task<Dictionary<string, string>?> GetInfoAsync(string userInfoEndpointUri, string token);
    }
}
