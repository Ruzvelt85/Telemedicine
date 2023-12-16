using Amazon.SQS.Model;
using Newtonsoft.Json;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Telemedicine.Common.Infrastructure.EventBus
{
    /// <summary>
    /// Build message for SQS by event
    /// </summary>
    public class SqsMessageRequestBuilder
    {
        /// <summary>
        /// Get SQS message request 
        /// </summary>
        /// <param name="event">Event</param>
        /// <param name="sqsUrl">Sql url</param>
        /// <param name="isFifo">Is FIFO queue?</param>
        /// <returns></returns>
        public static SendMessageRequest GetMessageRequest(IntegrationEvent @event, string? sqsUrl, bool? isFifo)
        {
            var messageRequest = new SendMessageRequest
            {
                QueueUrl = sqsUrl,
                MessageBody = JsonConvert.SerializeObject(@event, @event.GetType(), new JsonSerializerSettings()) // specify Settings just in order to use proper method overloading
            };

            SetMessageRequestAttributes(messageRequest, @event);

            if (isFifo!.Value)
            {
                messageRequest.MessageGroupId = @event is IIntegrationEventWithMessageGroupId msgGrpEvent ? msgGrpEvent.MessageGroupId : @event.GetType().Name;
                messageRequest.MessageDeduplicationId = @event.Id.ToString();
            }

            return messageRequest;
        }

        /// <summary>
        /// Set message request attributes
        /// </summary>
        /// <param name="messageRequest"></param>
        /// <param name="event"></param>
        private static void SetMessageRequestAttributes(SendMessageRequest messageRequest, IntegrationEvent @event)
        {
            messageRequest.MessageAttributes["Type"] = new MessageAttributeValue { StringValue = @event.GetType().Name, DataType = "String" };
            messageRequest.MessageAttributes["CorrelationId"] = new MessageAttributeValue { StringValue = @event.CorrelationId ?? "null", DataType = "String" };

            if (@event is IIntegrationEventWithEntityId eventWithEntityId)
            {
                messageRequest.MessageAttributes["EntityId"] = new MessageAttributeValue { StringValue = eventWithEntityId.EntityId.ToString(), DataType = "String" };
            }
        }
    }
}
