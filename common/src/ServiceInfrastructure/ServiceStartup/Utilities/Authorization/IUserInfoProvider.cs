using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    /// <summary>
    /// Provides additional user info.
    /// </summary>
    public interface IUserInfoProvider
    {
        /// <summary>
        /// Get the user info from a remote source. The user info is taken for a user whose <paramref name="token"/> is provided.
        /// </summary>
        /// <param name="token">token of the user</param>
        /// <returns> null if couldn't get the user info, otherwise returns Dictionary where Key is claim type, and value is claim value</returns>
        Task<Dictionary<string, string>?> GetUserInfoAsync(string token);
    }
}
