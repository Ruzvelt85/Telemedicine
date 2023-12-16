using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests.TestSetup
{
    public class Test3Entity : IEntity
    {
        public Guid Id { get; init; }

        public int HealthCenterId { get; set; }

        public string? Name { get; set; }

        public DateTime WorkStartDate { get; set; }
    }
}
