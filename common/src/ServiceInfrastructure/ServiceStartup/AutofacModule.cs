using Autofac;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup
{
    /// <summary>
    ///     Autofac module to put services into DI-container
    /// </summary>
    internal class AutofacModule : Module
    {
        public AutofacModule(ServiceStartupBase serviceStartup)
        {
            ServiceStartup = serviceStartup;
        }

        private ServiceStartupBase ServiceStartup { get; }


        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            ServiceStartup.RegisterServicesInIoC(builder);
        }
    }
}
