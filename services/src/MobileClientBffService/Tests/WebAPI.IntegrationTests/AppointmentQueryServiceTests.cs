using System;
using System.Threading.Tasks;
using Refit;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.MobileClientBffService.API.AppointmentQueryService;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class AppointmentQueryServiceTests : IHttpServiceTests<IAppointmentQueryService>
    {
        private readonly IAppointmentQueryService _service;

        /// <inheritdoc />
        public HttpServiceFixture<IAppointmentQueryService> HttpServiceFixture { get; }

        public AppointmentQueryServiceTests(HttpServiceFixture<IAppointmentQueryService> httpServiceFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            _service = HttpServiceFixture.GetRestService();
        }

        /// <summary>
        /// For test purpose we use here existing ID from data seed (AppointmentId = "C3F1B4E2-4C72-437D-933F-879E031DC629")
        /// </summary>
        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetConnectionInfo_ShouldReturnCorrectException()
        {
            await Assert.ThrowsAsync<ApiException>(() => _service.GetAppointmentConnectionInfoAsync(Guid.Parse("A85475D6-3364-4E2C-B1F7-0863469EFB88")));
        }
    }
}
