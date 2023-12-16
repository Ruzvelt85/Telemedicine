using System;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using JetBrains.Annotations;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender
{
    /// <summary>
    /// <inheritdoc cref="IEventBusPublisher"/>
    /// </summary>
    [PublicAPI]
    public class SqsEventBusPublisher : IEventBusPublisher
    {
        private readonly ISqsClientBuilder _sqsClientBuilder;
        private readonly SqsSettings _sqsConfig;
        private readonly ILogger _logger = Log.ForContext<SqsEventBusPublisher>();

        public SqsEventBusPublisher(ISqsClientBuilder sqsClientBuilder, IOptionsSnapshot<SqsSettings> sqsSettingsSnapshot)
        {
            _sqsClientBuilder = sqsClientBuilder;
            _sqsConfig = sqsSettingsSnapshot.Value;
        }

        /// <summary>
        /// Sends a message to Amazon SQS
        /// </summary>
        /// <param name="event">Information that will be send to the queue</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationValidationException">Thrown when configuration is not valid</exception>
        public async Task PublishAsync(IntegrationEvent @event)
        {
            await PublishInternalAsync(@event);
        }

        /// <summary>
        /// Sends a message to Amazon SQS in a safe manner
        /// </summary>
        /// <param name="event">Information that will be send to the queue</param>
        /// <returns></returns>
        public async Task<bool> TryPublishAsync(IntegrationEvent @event)
        {
            try
            {
                await PublishInternalAsync(@event);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An exception occurred while publishing a message to SQS");
                return false;
            }
        }

        private async Task PublishInternalAsync(IntegrationEvent @event)
        {
            using var sqsClient = _sqsClientBuilder.Build(_sqsConfig);

            SendMessageRequest sendMessageRequest = SqsMessageRequestBuilder.GetMessageRequest(@event, _sqsConfig.Url, _sqsConfig.IsFifo);
            SendMessageResponse responseSendMsg = await sqsClient.SendMessageAsync(sendMessageRequest);

            _logger.Information("SQS response: {Msg}", JsonConvert.SerializeObject(responseSendMsg));
        }
    }
}
