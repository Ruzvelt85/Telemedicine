using Microsoft.Extensions.DependencyInjection;
using Telemedicine.Common.Infrastructure.Refit;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Extensions
{
    public static class MvcBuilderExtension
    {
        /// <summary>
        /// Add and configure Json serializer
        /// </summary>
        public static IMvcBuilder ConfigureJsonSerializer(this IMvcBuilder mvcBuilder)
        {
            return mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ConfigureJsonSerializerSettings());
        }
    }
}
