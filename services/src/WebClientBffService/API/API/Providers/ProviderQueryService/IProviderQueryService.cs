using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Refit;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService
{
    public interface IProviderQueryService
    {
        [Get("/api/users/providers")]
        Task<PagedListResponseDto<ProviderResponseDto>> GetProviderListAsync(ProviderListRequestDto request, CancellationToken cancellationToken = default);
    }
}
