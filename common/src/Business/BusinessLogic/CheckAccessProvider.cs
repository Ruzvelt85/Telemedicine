using System;
using System.Threading.Tasks;

namespace Telemedicine.Common.Business.BusinessLogic
{
    public class CheckAccessProvider : ICheckAccessProvider
    {
        /// <inheritdoc />
        public Task ShouldHaveSameHealthCenterAsync(params Guid[] userIds) => Task.CompletedTask;
    }
}
