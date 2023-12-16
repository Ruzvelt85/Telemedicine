using System;
using System.Runtime.CompilerServices;
using Telemedicine.Common.Infrastructure.EntityBase;
using MediatR;

[assembly: InternalsVisibleTo("Telemedicine.Common.Infrastructure.Tests.DomainEventInfrastructureTests")]
namespace Telemedicine.Common.Infrastructure.DomainEventInfrastructure
{
    internal static class DomainEventNotificationFactory
    {
        public static INotification Create(DomainEvent domainEvent)
        {
            if (domainEvent is null)
            {
                throw new ArgumentNullException(nameof(domainEvent));
            }

            var domainEventNotificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());

            var domainEventNotification = Activator.CreateInstance(domainEventNotificationType, domainEvent);

            if (domainEventNotification is INotification notification)
            {
                return notification;
            }

            throw new InvalidCastException($"Cannot convert a {typeof(DomainEventNotification<>)} to a {typeof(INotification)}.");
        }
    }
}
