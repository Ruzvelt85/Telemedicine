using System;
using Amazon.Runtime;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase;

// ReSharper disable UnusedMember.Global

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender
{
    /// <summary>
    /// Config to configure <see cref="Amazon.SQS.AmazonSQSClient"/>
    /// </summary>
    public record SqsSettings
    {
        public static SqsSettings Default = new();

        [DataLogMasked]
        public string? Url { get; init; }

        /// <summary>
        /// true if the SQS type is FIFO, false if it's Standard
        /// </summary>
        public bool? IsFifo { get; init; }

        [DataLogMasked]
        public string? AccessKey { get; init; }

        [DataLogMasked]
        public string? SecretKey { get; init; }

        /// <inheritdoc cref="AmazonSqsClientSettings"/>
        public AmazonSqsClientSettings? AmazonConfiguration { get; init; }

        /// <summary>
        /// Contains configuration for <see cref="Amazon.SQS.AmazonSQSClient" />. Duplicates values for <see cref="Amazon.SQS.AmazonSQSConfig"/>
        /// Only primitive types properties are available. If you need complex types properties you may add them into the class
        /// </summary>
        public record AmazonSqsClientSettings
        {
            public string? RegionEndpoint { get; init; }
            public SigningAlgorithm? SignatureMethod { get; init; }
            public string? SignatureVersion { get; init; }
            public bool? UseAlternateUserAgentHeader { get; init; }
            // ReSharper disable once InconsistentNaming
            public string? ServiceURL { get; init; }
            public bool? UseHttp { get; init; }
            public string? AuthenticationRegion { get; init; }
            public string? AuthenticationServiceName { get; init; }
            public int? MaxErrorRetry { get; init; }
            public bool? LogResponse { get; init; }
            public int? BufferSize { get; init; }
            public long? ProgressUpdateInterval { get; init; }
            public bool? ResignRetries { get; init; }
            public bool? AllowAutoRedirect { get; init; }
            public bool? LogMetrics { get; init; }
            public bool? DisableLogging { get; init; }
            public TimeSpan? Timeout { get; init; }
            // ReSharper disable once IdentifierTypo
            public bool? UseDualstackEndpoint { get; init; }
            public bool? ThrottleRetries { get; init; }
            public bool? DisableHostPrefixInjection { get; init; }
            public bool? EndpointDiscoveryEnabled { get; init; }
            public int? EndpointDiscoveryCacheLimit { get; init; }
            public RequestRetryMode? RetryMode { get; init; }
            public bool? FastFailRequests { get; init; }
            public bool? CacheHttpClient { get; init; }
            public int? HttpClientCacheSize { get; init; }
            public string? ProxyHost { get; init; }
            public int? ProxyPort { get; init; }
            public int? MaxConnectionsPerServer { get; init; }
        }
    }
}
