using System;
using Autofac;

namespace Telemedicine.Common.Infrastructure.VidyoClient.StartupSetupExtensions
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddVidyoService(this ContainerBuilder builder)
        {
            if (builder is null)
            { throw new ArgumentNullException(nameof(builder)); }

            builder.RegisterType<VidyoClientHealthCheck>().SingleInstance();
            builder.RegisterType<VidyoClientFactory>().As<IVidyoClientFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VidyoClient>().As<IVidyoClient>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
