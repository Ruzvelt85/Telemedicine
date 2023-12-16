using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests.TestSetup
{
    public class Test4Entity : IEntity
    {
        public Guid Id { get; init; }

        public string? Name { get; set; }
    }
}
