using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.CorrelationId;
using Telemedicine.Services.AppointmentDomainService.API.IntegrationEvents;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Core.Events;
using Telemedicine.Services.AppointmentDomainService.WebAPI.EventHandlers;
using Moq;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.EventHandlers
{
    public class AppointmentCreatedEventHandlerTests
    {
        [Fact]
        public async Task Handle_WhenAppointmentCreatedDomainEvent_ShouldPublishAppointmentCancelledIntegrationEvent()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.Status, AppointmentStatus.Opened)
                .With(_ => _.IsDeleted, false)
                .Create();
            const string correlationId = "CorrelationId";

            var domainEvent = new AppointmentCreatedDomainEvent(appointment);
            var notification = new DomainEventNotification<AppointmentCreatedDomainEvent>(domainEvent);

            var eventBusPublisherSpy = new EventBusPublisherSpy<AppointmentCreatedIntegrationEvent>();

            var correlationIdAccessorMock = new Mock<ICorrelationIdAccessor>();
            correlationIdAccessorMock
                .Setup(_ => _.CorrelationId)
                .Returns(correlationId);

            var handler = new AppointmentCreatedEventHandler(eventBusPublisherSpy, correlationIdAccessorMock.Object);

            // Act
            await handler.Handle(notification, CancellationToken.None);

            // Assert
            eventBusPublisherSpy.ShouldPublishNumberOfEvents(1)
                .And.ShouldContain(_ => _.AppointmentId == appointmentId
                                                    && _.CorrelationId == correlationId);
        }
    }
}
