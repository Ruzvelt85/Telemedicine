using System;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Common.Business.BusinessLogic
{
    public interface ICurrentUserProvider
    {
        /// <summary>
        /// Gets the current user from claims that we got from a JWT 
        /// </summary>
        /// <exception cref="UserClaimException">Thrown when no specified claim exists or the value of it is invalid</exception>
        Guid GetId();
    }
}
