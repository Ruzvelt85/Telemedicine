using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    public class TestLogicallyDeletableAuditableWriteRepository : WriteRepository<TestLogicallyDeletedAuditableEntity>
    {
        public TestLogicallyDeletableAuditableWriteRepository(TestDbContext context) : base(context)
        {

        }
    }
}
