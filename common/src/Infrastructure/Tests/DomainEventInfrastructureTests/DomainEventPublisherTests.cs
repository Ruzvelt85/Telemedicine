using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests.TestSetup;
using MediatR;
using Moq;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests
{
    public class DomainEventPublisherTests
    {
        [Fact]
        public void PublishAsync_WhenCreatesNotification_ShouldPublishDomainEvent()
        {
            // Arrange
            var domainEventId = Guid.NewGuid();
            var domainEvent = new TestDomainEvent(domainEventId);

            var capturedMediatorParams = new List<INotification>();

            var mediatorMock = new Mock<IPublisher>();
            mediatorMock
                .Setup(_ => _.Publish(Capture.In(capturedMediatorParams), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var domainEventPublisher = new DomainEventPublisher(mediatorMock.Object);

            // Act
            Func<Task> func = async () => await domainEventPublisher.PublishAsync(domainEvent);

            // Assert
            func.Should().NotThrowAsync();
            mediatorMock.Verify();

            capturedMediatorParams.Should().NotBeEmpty()
                .And.HaveCount(1)
                .And.ContainItemsAssignableTo<DomainEventNotification<TestDomainEvent>>();

            var notification = capturedMediatorParams[0];
            var domainEventNotification = (DomainEventNotification<TestDomainEvent>)notification;
            domainEventNotification.DomainEvent.Should().NotBeNull();
            domainEventNotification.DomainEvent.Id.Should().Be(domainEventId);
        }
    }
}
