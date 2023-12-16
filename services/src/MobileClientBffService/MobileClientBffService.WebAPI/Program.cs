using System.Threading.Tasks;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;

namespace Telemedicine.Services.MobileClientBffService.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args) => await ServiceStartupBase.BuildHost<Startup>(args);

    }
}
