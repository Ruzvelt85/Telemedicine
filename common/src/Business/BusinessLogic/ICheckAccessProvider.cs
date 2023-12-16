using System;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;

namespace Telemedicine.Common.Business.BusinessLogic
{
    public interface ICheckAccessProvider
    {
        /// <summary>
        /// Check that users belong to same health center
        /// </summary>
        /// <exception cref="BusinessException">Thrown when users health centers do not match.</exception>
        Task ShouldHaveSameHealthCenterAsync(params Guid[] userIds);
    }
}
