using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.MigrationEfCoreCommonTests.TestSetup
{
    public class Test2Entity : IEntity
    {
        public Guid Id { get; init; }

        public string? Name { get; set; }

        public string? MedicalRecords { get; set; }

        public float Weight { get; set; }

        public byte Age { get; set; }
    }
}
