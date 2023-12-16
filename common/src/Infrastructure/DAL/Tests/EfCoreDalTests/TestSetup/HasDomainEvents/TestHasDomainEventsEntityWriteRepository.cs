using Telemedicine.Common.Infrastructure.DAL.EfCoreDal.Repositories;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.HasDomainEvents
{
    /// <summary>
    /// Test Write Repository for entity that implement IHasDomainEvent
    /// </summary>
    public class TestHasDomainEventsEntityWriteRepository : WriteRepository<TestHasDomainEventsEntity>
    {
        public TestHasDomainEventsEntityWriteRepository(TestDbContext context) : base(context)
        {
        }
    }
}
