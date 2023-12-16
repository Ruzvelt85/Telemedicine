using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    public class TestLogicallyDeletableReadRepository : ReadRepository<TestLogicallyDeletedEntity>
    {
        public TestLogicallyDeletableReadRepository(TestDbContext context) : base(context)
        {

        }
    }
}
