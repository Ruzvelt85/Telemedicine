using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Simple
{
    /// <summary>
    /// Test Write Repository for TestSimpleEntity
    /// </summary>
    public class TestSimpleEntityWriteRepository : WriteRepository<TestSimpleEntity>
    {
        public TestSimpleEntityWriteRepository(TestDbContext context) : base(context)
        {
        }
    }
}
