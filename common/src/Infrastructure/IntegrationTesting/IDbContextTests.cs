using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Xunit;

namespace Telemedicine.Common.Infrastructure.IntegrationTesting
{
    public interface IDbContextTests<TDbContext> : IClassFixture<EfCoreContextFixture<TDbContext>> where TDbContext : EfCoreDbContext
    {
        public EfCoreContextFixture<TDbContext> EfCoreContextFixture { get; }
    }
}
