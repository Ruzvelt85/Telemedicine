using System;
using System.Threading.Tasks;
using Xunit;
using Refit;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService;
using Telemedicine.Services.VideoConfIntegrService.DAL;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class ConferenceCommandServiceTests : IDisposable, IHttpServiceTests<IVideoConferenceCommandService>, IDbContextTests<VideoConferenceIntegrationServiceDbContext>
    {
        /// <inheritdoc />
        public HttpServiceFixture<IVideoConferenceCommandService> HttpServiceFixture { get; }

        /// <inheritdoc />
        public EfCoreContextFixture<VideoConferenceIntegrationServiceDbContext> EfCoreContextFixture { get; }

        public IVideoConferenceCommandService Service { get; }

        public VideoConferenceIntegrationServiceDbContext DbContext { get; }

        public ConferenceCommandServiceTests(
            HttpServiceFixture<IVideoConferenceCommandService> httpServiceFixture,
            EfCoreContextFixture<VideoConferenceIntegrationServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            Service = HttpServiceFixture.GetRestService();
            DbContext = EfCoreContextFixture.DbContext;
        }

        [Fact]
        public async Task CreateConference_DefaultRequest_ShouldThrowValidationApiException()
        {
            var request = new CreateConferenceRequestDto();
            var exception = await Record.ExceptionAsync(() => Service.CreateAsync(request));

            Assert.NotNull(exception);
            Assert.IsType<ValidationApiException>(exception);
        }

        [Fact]
        public async Task CreateConference_PastDate_ShouldThrowValidationApiException()
        {
            // Arrange
            var request = new CreateConferenceRequestDto
            {
                AppointmentId = Guid.NewGuid(),
                AppointmentStartDate = DateTime.UtcNow.AddDays(-1),
                AppointmentDuration = TimeSpan.Zero,
                AppointmentTitle = ""
            };

            // Act
            var exception = await Record.ExceptionAsync(() => Service.CreateAsync(request));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationApiException>(exception);
        }

        [Fact(Skip = "This test uses a real request to Vidyo service")]
        public async Task CreateConference_ShouldCreate()
        {
            // Arrange
            var tomorrowDate = DateTime.UtcNow.AddDays(1);
            var createConferenceRequest = new CreateConferenceRequestDto
            {
                AppointmentId = Guid.NewGuid(),
                AppointmentTitle = "Regular call",
                AppointmentStartDate = new DateTime(tomorrowDate.Year, tomorrowDate.Month, tomorrowDate.Day, tomorrowDate.Hour, tomorrowDate.Minute, tomorrowDate.Second),
                AppointmentDuration = TimeSpan.FromHours(1)
            };

            // Act
            await Service.CreateAsync(createConferenceRequest);

            // Assert
            var conference = await DbContext.Conferences.AsNoTracking().FirstOrDefaultAsync(_ => _.AppointmentId == createConferenceRequest.AppointmentId);
            Assert.NotNull(conference);
            Assert.Equal(conference.AppointmentDuration, createConferenceRequest.AppointmentDuration);
            Assert.Equal(conference.AppointmentStartDate, createConferenceRequest.AppointmentStartDate);
            Assert.Equal(conference.AppointmentTitle, createConferenceRequest.AppointmentTitle);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
