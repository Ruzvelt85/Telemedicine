using System;
using Autofac;
using Telemedicine.Common.Infrastructure.Patterns.EventBus;

namespace Telemedicine.Common.Infrastructure.EventBus.SqsSender
{
    /// <summary>
    /// Extension methods for configuring HttpContext services.
    /// </summary>
    public static class SqsSenderServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a default implementation for the <see cref="IEventBusPublisher"/> service.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/>.</param>
        /// <returns>The container builder.</returns>
        public static ContainerBuilder AddSqsSender(this ContainerBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            /* must be registered as Scoped because it takes IOptionsSnapshot as a parameter */
            builder.RegisterType<SqsEventBusPublisher>().As<IEventBusPublisher>().InstancePerLifetimeScope();
            builder.RegisterType<SqsClientBuilder>().As<ISqsClientBuilder>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
