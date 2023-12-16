using System;
using System.IO;
using Amazon.SQS;
using Amazon.SQS.Model;
using Telemedicine.Common.Infrastructure.EventBus.SqsSender;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Telemedicine.Common.Infrastructure.EventBus
{
    public static class SqsHealthCheckBuilderExtensions
    {
        private static readonly SqsClientBuilder _clientBuilder = new(); //singleton specially for the health checker

        /// <summary>
        /// Checks that the specified queue exists
        /// </summary>
        /// <param name="healthCheckBuilder"></param>
        /// <param name="settings">settings for <see cref="AmazonSQSClient"/></param>
        public static void AddSqs(this IHealthChecksBuilder healthCheckBuilder, SqsSettings settings)
        {
            healthCheckBuilder.AddAsyncCheck("SqsHealthCheck", async () =>
            {
                try
                {
                    using AmazonSQSClient sqsClient = _clientBuilder.Build(settings); //TechDebt: to reuse the client. Check how to add HttpClientFactory to it's config.
                    string queueName = Path.GetFileName(settings.Url!); //the queue name is in the end of its URL, example: https://..../dev1-base

                    await sqsClient.GetQueueUrlAsync(queueName); //SQS doesn't have a specific method for health check. It's a common practice to use GetQueueUrlAsync for simple liveliness test, mainly because it throws self-descriptive QueueDoesNotExistException if the queue couldn't be found.
                }
                catch (QueueDoesNotExistException ex)
                {
                    return HealthCheckResult.Unhealthy($"The queue does not exist: {settings.Url!}", ex);
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"Unexpected exception occurred while connecting to: {settings.Url!}", ex);
                }

                return HealthCheckResult.Healthy();
            });
        }
    }
}
