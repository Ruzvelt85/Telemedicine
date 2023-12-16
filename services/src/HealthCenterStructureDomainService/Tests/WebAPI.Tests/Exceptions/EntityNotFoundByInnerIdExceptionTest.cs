using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.Exceptions
{
    public class EntityNotFoundByInnerIdExceptionTest
    {
        [Fact]
        public void EntityNotFoundException_Test()
        {
            const string innerId = "1234";
            var type = typeof(EntityNotFoundByInnerIdExceptionTest);
            void ThrowException() => throw new EntityNotFoundByInnerIdException(type, innerId);

            var ex = (EntityNotFoundByInnerIdException)Record.Exception(ThrowException)!;

            Assert.NotNull(ex);
            Assert.IsType<EntityNotFoundByInnerIdException>(ex);
            Assert.Equal(3, ex.Data.Count);
            Assert.True(ex.Data.Contains(nameof(ex.Code)));
            Assert.True(ex.Data.Contains(nameof(ex.Type)));
            Assert.True(ex.Data.Contains(nameof(ex.InnerId)));

            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), ex.Data[nameof(ex.Code)]);
            Assert.Equal(type.FullName, ex.Data[nameof(ex.Type)]);
            Assert.Equal(innerId, ex.Data[nameof(ex.InnerId)]);
        }
    }
}
