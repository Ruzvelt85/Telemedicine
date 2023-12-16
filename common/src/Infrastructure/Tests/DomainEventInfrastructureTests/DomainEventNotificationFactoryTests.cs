using System;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests.TestSetup;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests
{
    public class DomainEventNotificationFactoryTests
    {
        [Fact]
        public void Create_WhenDomainEventIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            DomainEvent domainEvent = default!;
            const string expectedParameterName = "domainEvent";

            // Act
            Action act = () => DomainEventNotificationFactory.Create(domainEvent);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName(expectedParameterName);
        }

        [Fact]
        public void Create_WhenDomainEvent_ShouldReturnExpected()
        {
            // Arrange
            var domainEvent = new TestDomainEvent(Guid.Empty);
            var expectedDomainEventNotification = new DomainEventNotification<TestDomainEvent>(domainEvent);

            //var domainEventNotificationFactory = new DomainEventNotificationFactory();

            // Act
            var result = DomainEventNotificationFactory.Create(domainEvent);

            // Assert
            result.Should().NotBeNull().And.BeEquivalentTo(expectedDomainEventNotification);
        }
    }
}
