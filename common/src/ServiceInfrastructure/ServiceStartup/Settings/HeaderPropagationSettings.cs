using System.Collections.Generic;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings
{
    public record HeaderPropagationSettings
    {
        public static HeaderPropagationSettings Default = new();

        /// <summary>
        /// <para>True</para>, if it is enabled
        /// </summary>
        public bool IsEnabled { get; init; } = false;

        /// <summary>
        /// Settings for <see cref="Microsoft.AspNetCore.HeaderPropagation.HeaderPropagationMiddleware"/>
        /// </summary>
        public IEnumerable<InboundPropagationHeader>? InboundHeaders { get; init; }

        /// <summary>
        /// Settings for <see cref="Microsoft.AspNetCore.HeaderPropagation.HeaderPropagationMessageHandler"/>
        /// </summary>
        public IEnumerable<OutboundPropagationHeader>? OutboundHeaders { get; init; }

        /// <summary>
        /// Settings for <see cref="Microsoft.AspNetCore.HeaderPropagation.HeaderPropagationMiddleware"/>
        /// </summary>
        public record InboundPropagationHeader
        {
            /// <summary>
            /// The name of the header in the incoming request containing the required value (to be "captured")
            /// </summary>
            public string? InboundHeaderName { get; init; }

            /// <summary>
            /// Name of the property to which you want to write a value "captured" from<see cref="InboundHeaderName"/>
            /// </summary>
            public string? CapturedHeaderName { get; init; }
        }

        /// <summary>
        /// Settings for <see cref="Microsoft.AspNetCore.HeaderPropagation.HeaderPropagationMessageHandler"/>
        /// </summary>
        public record OutboundPropagationHeader
        {
            /// <summary>
            /// Name of the property where the header value was "captured" in the current request
            /// </summary>
            public string? CapturedHeaderName { get; init; }

            /// <summary>
            /// Name of the header to which you want to write a value stored in the property <see cref="CapturedHeaderName"/>
            /// </summary>
            public string? OutboundHeaderName { get; init; }
        }
    }
}
