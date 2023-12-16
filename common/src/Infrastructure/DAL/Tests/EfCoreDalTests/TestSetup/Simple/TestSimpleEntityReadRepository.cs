using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Simple
{
    /// <summary>
    /// Test Read Repository for TestSimpleEntity
    /// </summary>
    public class TestSimpleEntityReadRepository : ReadRepository<TestSimpleEntity>
    {
        public TestSimpleEntityReadRepository(TestDbContext context) : base(context)
        {
        }
    }
}
