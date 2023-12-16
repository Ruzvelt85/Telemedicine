using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Telemedicine.Common.ServiceInfrastructure.Tests.StartupTests.Setup
{
    public interface ITestServiceApi
    {
        [Post("/api/entities")]
        Task<TestServiceApiResponseDto> GetTestResponse([Body(buffered: true)] TestServiceApiRequestDto request, CancellationToken cancellationToken = default);
    }
}
