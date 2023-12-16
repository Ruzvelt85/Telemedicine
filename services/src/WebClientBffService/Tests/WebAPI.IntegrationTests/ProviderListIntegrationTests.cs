using System;
using System.Linq;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService;
using Telemedicine.Services.WebClientBffService.API.Providers.ProviderQueryService.Dto;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class ProviderListIntegrationTests : IHttpServiceTests<IProviderQueryService>
    {
        private readonly IProviderQueryService _service;
        public HttpServiceFixture<IProviderQueryService> HttpServiceFixture { get; }

        public ProviderListIntegrationTests(HttpServiceFixture<IProviderQueryService> httpServiceFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            _service = HttpServiceFixture.GetRestService();
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetProviderList_CommonSuccessfulIntegrationTest()
        {
            var healthCenterId = Guid.Parse("52A0218C-70F4-4B9B-A9B3-1C013D04D4C9");
            var request = new ProviderListRequestDto
            {
                Paging = new PagingRequestDto(),
                Filter = new ProviderListFilterRequestDto
                {
                    HealthCenterIds = new[] { healthCenterId }
                }
            };

            PagedListResponseDto<ProviderResponseDto> providerResponse = await _service.GetProviderListAsync(request);

            Assert.NotNull(providerResponse);
            Assert.NotNull(providerResponse.Paging);
            Assert.NotNull(providerResponse.Items);
            Assert.NotEmpty(providerResponse.Items);

            var providers = providerResponse.Items.FirstOrDefault(x =>
                x.HealthCenters.Any(p => p.Id == healthCenterId));

            Assert.NotNull(providers);
        }
    }
}
