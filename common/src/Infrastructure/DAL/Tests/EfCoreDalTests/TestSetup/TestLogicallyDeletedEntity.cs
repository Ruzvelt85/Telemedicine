using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestLogicallyDeletedEntity : IEntity, ILogicallyDeletable
    {
        public Guid Id { get; set; }

        public byte Age { get; set; }

        public bool IsDeleted { get; set; }
    }
}
