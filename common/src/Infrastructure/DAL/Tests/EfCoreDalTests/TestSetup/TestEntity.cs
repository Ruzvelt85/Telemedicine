using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    public class TestEntity : IEntity
    {
        public Guid Id { get; init; }

        public string? Name { get; set; }

        public byte Age { get; set; }

        public DateTime BirthDate { get; set; }

        public TimeSpan TimeSinceLastCall { get; set; }
    }
}
