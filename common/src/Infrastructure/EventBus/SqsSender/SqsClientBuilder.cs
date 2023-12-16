using Amazon.Runtime;
using Amazon.SQS;
using Serilog;
using Telemedicine.Common.Infrastructure.EventBus.SqsSender.Mappings;

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender
{
    ///<inheritdoc cref="ISqsClientBuilder"/>
    internal class SqsClientBuilder : ISqsClientBuilder
    {
        private readonly ILogger _logger = Log.ForContext<SqsClientBuilder>();

        ///<inheritdoc cref="ISqsClientBuilder.Build"/>
        public AmazonSQSClient Build(SqsSettings sqsConfig)
        {
            var credentials = new BasicAWSCredentials(sqsConfig.AccessKey, sqsConfig.SecretKey);
            var amazonConfig = new AmazonSQSConfig(); //We need to create the object explicitly before using the mapper in order to default values be set.

            SqsMapping.Mapper.Map(sqsConfig.AmazonConfiguration, amazonConfig);
            //amazonConfig.HttpClientFactory //TechDebt: perhaps, we can set the client factory for it but I couldn't find any documentation about it

            return new AmazonSQSClient(credentials, amazonConfig);
        }
    }
}
