using System.Threading;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService
{
    public interface IMobileClientQueryService
    {
        [Get("/api/data")]
        Task<LastChangedDataResponseDto> GetLastChangedData(LastChangedDataRequestDto request, CancellationToken cancellationToken = default);
    }
}
