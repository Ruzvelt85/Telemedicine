using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    /// <summary>
    /// Test Write Repository for TestEntity
    /// </summary>
    public class TestEntityWriteRepository : WriteRepository<TestEntity>
    {
        public TestEntityWriteRepository(TestDbContext context) : base(context)
        {
        }
    }
}
