using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    /// <summary>
    /// Test Read Repository for TestEntity
    /// </summary>
    public class TestEntityReadRepository : ReadRepository<TestEntity>
    {
        public TestEntityReadRepository(TestDbContext context) : base(context)
        {
        }
    }
}
