using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Refit;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public class AppointmentQueryServiceTests : IDisposable,
        IHttpServiceTests<IAppointmentQueryService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        /// <inheritdoc />
        public HttpServiceFixture<IAppointmentQueryService> HttpServiceFixture { get; }

        /// <inheritdoc />
        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        public AppointmentDomainServiceDbContext DbContext { get; }

        public IAppointmentQueryService Service { get; }

        public AppointmentQueryServiceTests(HttpServiceFixture<IAppointmentQueryService> httpServiceFixture, EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_ShouldReturnCorrectAppointment()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();

            var appointments = new List<Appointment>
            {
                GetAppointment(appointmentId, Guid.NewGuid(), new DateTime(2022, 11, 2), TimeSpan.FromMinutes(45)),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 2, 7), TimeSpan.FromMinutes(25)),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 4, 6), TimeSpan.FromMinutes(30)),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 29), TimeSpan.FromMinutes(5))
            };

            await DbContext.AddRangeAsync(appointments);
            await DbContext.SaveChangesAsync();

            // Act
            var response = await Service.GetAppointmentByIdAsync(appointmentId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(appointmentId, response.Id);
            Assert.Equal(appointmentId, response.Id);
            Assert.Equal(appointments[0].Title, response.Title);
            Assert.Equal(appointments[0].Description, response.Description);
            Assert.Equal(appointments[0].StartDate, response.StartDate);
            Assert.Equal(appointments[0].Duration, response.Duration);
            Assert.Equal(appointments[0].Type.ToString(), response.Type.ToString());
            Assert.Equal(appointments[0].GetState().ToString(), response.State.ToString());
            Assert.Equal(appointments[0].Rating, response.Rating);

            Assert.Equal(appointments[0].Attendees.Count, response.Attendees.Count);
            Assert.Equal(appointments[0].Attendees.First().UserId, response.Attendees.First());
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_NoAppointment_ShouldThrowApiException()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 11, 2), TimeSpan.FromMinutes(45)),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 2, 7), TimeSpan.FromMinutes(25)),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 4, 6), TimeSpan.FromMinutes(30)),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2022, 7, 29), TimeSpan.FromMinutes(5))
            };
            await DbContext.AddRangeAsync(appointments);
            await DbContext.SaveChangesAsync();

            // Act
            var exception = await Record.ExceptionAsync(() => Service.GetAppointmentByIdAsync(Guid.NewGuid()));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        private static Appointment GetAppointment(Guid appointmentId, Guid patientId, DateTime startDate, TimeSpan duration)
        {
            return new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.Attendees, new List<AppointmentAttendee>()
                {
                    new Fixture().Build<AppointmentAttendee>()
                        .With(_ => _.AppointmentId, appointmentId)
                        .With(_ => _.UserId, patientId)
                        .With(_ => _.IsDeleted, false)
                        .Create(),
                })
                .Create();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
