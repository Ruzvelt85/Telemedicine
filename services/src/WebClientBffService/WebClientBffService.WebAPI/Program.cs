using System.Threading.Tasks;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;

namespace Telemedicine.Services.WebClientBffService.WebAPI
{
    public static class Program
    {
        public static async Task Main(string[] args) => await ServiceStartupBase.BuildHost<Startup>(args);
    }
}
