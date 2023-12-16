using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;

namespace Telemedicine.Common.Infrastructure.Patterns.EventBus
{
    /// <summary>
    /// Class that can publish an event to a message queue.
    /// Needs to be injected into services as Lazy because it's not required for every request  
    /// </summary>
    public interface IEventBusPublisher
    {
        /// <summary>
        /// Sends a message to a queue
        /// </summary>
        /// <param name="event">Information that will be send to the queue</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationException">Thrown when configuration is not valid</exception>
        Task PublishAsync(IntegrationEvent @event);

        /// <summary>
        /// Sends a message to a queue in a safe manner
        /// </summary>
        /// <param name="event">Information that will be send to the queue</param>
        /// <returns></returns>
        Task<bool> TryPublishAsync(IntegrationEvent @event);
    }
}
