using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.Patterns.EventBus
{
    [PublicAPI]
    public interface IIntegrationEventWithMessageGroupId
    {
        public string MessageGroupId { get; set; }
    }
}
