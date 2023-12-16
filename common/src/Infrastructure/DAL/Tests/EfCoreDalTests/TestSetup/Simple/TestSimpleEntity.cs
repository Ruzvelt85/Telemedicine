using System;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Simple
{
    public record TestSimpleEntity(Guid Id) : IEntity;
}
