using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestLogicallyDeletedAuditableEntity : IEntity, ILogicallyDeletable, IAuditable, IOptimisticLock
    {
        public Guid Id { get; set; }

        public string? Test { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public uint Timestamp { get; }
    }
}
