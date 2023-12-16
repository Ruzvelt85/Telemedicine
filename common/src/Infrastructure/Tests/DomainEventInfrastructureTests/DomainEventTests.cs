using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;
using Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests.TestSetup;
using MediatR;
using Moq;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests
{
    public class DomainEventTests
    {
        [Fact]
        public async Task PublishAsync_WhenPublishedDomainEvent_ShouldPublishIntegrationEvent()
        {
            // Arrange
            var eventBusPublisherMock = new Mock<IEventBusPublisher>();
            var correlationIdAccessorMock = new Mock<ICorrelationIdAccessor>();
            var mediator = GetMediator(eventBusPublisherMock.Object, correlationIdAccessorMock.Object);

            var entityId = Guid.NewGuid();
            var domainEvent = new TestDomainEvent(entityId);

            var domainEventPublisher = new DomainEventPublisher(mediator);

            // Act
            await domainEventPublisher.PublishAsync(domainEvent);

            // Assert
            eventBusPublisherMock.Verify(_ => _.TryPublishAsync(It.IsAny<TestIntegrationEvent>()), Times.Once);
        }

        private static IPublisher GetMediator(IEventBusPublisher eventBusPublisher, ICorrelationIdAccessor correlationIdAccessor)
        {
            var testHandler = new TestDomainEventHandler(eventBusPublisher, correlationIdAccessor);

            var mediatorMock = new Mock<IPublisher>();
            mediatorMock
                .Setup(_ => _.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Callback((INotification notification, CancellationToken token) => HandleAsync(notification, token))
                .Returns(Task.CompletedTask);

            return mediatorMock.Object;

            async void HandleAsync(INotification notification, CancellationToken token)
            {
                if (notification is DomainEventNotification<TestDomainEvent> testNotification)
                {
                    await testHandler.Handle(testNotification, token);
                }
            }
        }
    }
}
