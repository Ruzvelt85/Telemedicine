using Autofac;
using JetBrains.Annotations;

namespace Telemedicine.Common.Infrastructure.SftpClient.StartupSetupExtensions
{
    [PublicAPI]
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Adds an implementation for the <see cref="ISftpClient"/> service.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/>.</param>
        /// <returns>The container builder.</returns>
        public static ContainerBuilder AddSftpClient(this ContainerBuilder builder)
        {
            builder.RegisterType<SftpHealthCheck>().SingleInstance();
            builder.RegisterType<SftpClientImpl>().As<ISftpClient>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalSftpClientFactory>().As<IExternalSftpClientFactory>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
