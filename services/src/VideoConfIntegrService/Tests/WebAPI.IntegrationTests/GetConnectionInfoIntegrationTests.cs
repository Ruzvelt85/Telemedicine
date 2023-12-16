using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.DAL;
using Telemedicine.Services.VideoConfIntegrService.Tests.Tests.Core;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class GetConnectionInfoIntegrationTests
        : IDisposable, IHttpServiceTests<IVideoConferenceQueryService>, IDbContextTests<VideoConferenceIntegrationServiceDbContext>
    {
        public HttpServiceFixture<IVideoConferenceQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<VideoConferenceIntegrationServiceDbContext> EfCoreContextFixture { get; }
        public VideoConferenceIntegrationServiceDbContext DbContext { get; }
        public IVideoConferenceQueryService Service { get; }

        public GetConnectionInfoIntegrationTests(
            HttpServiceFixture<IVideoConferenceQueryService> httpServiceFixture,
            EfCoreContextFixture<VideoConferenceIntegrationServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetConnectionInfoByAppointmentId_ShouldReturnCorrectConnectionInfo()
        {
            // Arrange
            var conferenceToFind = Fixtures.CreateConferenceFixture(
                Guid.NewGuid(),
                "test",
                123,
                "https://test.com/thd",
                "12345");

            var secondConference = Fixtures.CreateConferenceFixture(
                Guid.NewGuid(),
                "test 2",
                987,
                "https://abc.com/kdj",
                "54321");

            var conferences = new List<Conference>()
            {
                conferenceToFind,
                secondConference
            };

            await DbContext.AddRangeAsync(conferences);
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Service.GetConnectionInfoAsync(conferenceToFind.AppointmentId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(conferenceToFind.AppointmentId, response.AppointmentId);
            Assert.Equal(conferenceToFind.RoomId, response.RoomId);
            Assert.Equal("https://test.com", response.Host);
            Assert.Equal("thd", response.RoomKey);
            Assert.Equal(conferenceToFind.RoomPin, response.RoomPin);
        }

        [Fact]
        public async Task GetConnectionInfoByAppointmentId_EmptyRepository_ShouldThrowException()
        {
            // Arrange
            var conference = Fixtures.CreateConferenceFixture(
                Guid.NewGuid(),
                "test",
                123,
                "https://test.com/",
                "12345");

            await DbContext.AddRangeAsync(conference);
            await DbContext.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => Service.GetConnectionInfoAsync(Guid.NewGuid()));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
