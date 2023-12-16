using Amazon.SQS;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender
{
    /// <summary>
    /// Builder for <see cref="AmazonSQSClient"/>
    /// </summary>
    public interface ISqsClientBuilder
    {
        /// <summary>
        /// Builds a configured object of <see cref="AmazonSQSClient"/>
        /// </summary>
        /// <param name="sqsConfig">Config that is used for creating an object of <see cref="AmazonSQSClient"/></param>
        /// <returns>Configured <see cref="AmazonSQSClient"/></returns>
        /// <exception cref="ConfigurationValidationException">Thrown when the provided config is not valid</exception>
        AmazonSQSClient Build(SqsSettings sqsConfig);
    }
}
