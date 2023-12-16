using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using AppointmentStatus = Telemedicine.Services.AppointmentDomainService.Core.Enums.AppointmentStatus;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class WebClientGetAppointmentListStatesTests
        : IDisposable, IHttpServiceTests<IWebClientBffQueryService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        public HttpServiceFixture<IWebClientBffQueryService> HttpServiceFixture { get; }

        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        public AppointmentDomainServiceDbContext DbContext { get; }
        public IWebClientBffQueryService Service { get; }

        private readonly Guid _doctorId;
        private readonly Guid[] _appointmentIds;

        public WebClientGetAppointmentListStatesTests(
            HttpServiceFixture<IWebClientBffQueryService> httpServiceFixture,
            EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;

            _doctorId = Guid.NewGuid();
            _appointmentIds = Enumerable.Range(0, 16)
                .Select(_ => Guid.NewGuid())
                .ToArray();

            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetAppointmentList_ShouldSelectTwoOpenAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId,
                    AppointmentStates = new[] { AppointmentState.Opened },
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            //Assert
            AssertResponse(response, _appointmentIds[9], _appointmentIds[10]);
        }

        [Fact]
        public async Task GetAppointmentList_ShouldSelectDoneAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId,
                    AppointmentStates = new[] { AppointmentState.Done },
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            //Assert
            AssertResponse(response, _appointmentIds[0], _appointmentIds[1]);
        }

        [Fact]
        public async Task GetAppointmentList_ShouldSelectOngoingAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId,
                    AppointmentStates = new[] { AppointmentState.Ongoing },
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            // Assert
            AssertResponse(response, _appointmentIds[7]);
        }

        [Fact]
        public async Task GetAppointmentList_ShouldSelectCancelledAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId,
                    AppointmentStates = new[] { AppointmentState.Cancelled },
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            // Assert
            AssertResponse(response, _appointmentIds[11]);
        }

        [Fact]
        public async Task GetAppointmentList_ShouldSelectMissedAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId,
                    AppointmentStates = new[] { AppointmentState.Missed },
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            // Assert
            AssertResponse(response, _appointmentIds[13], _appointmentIds[14]);
        }

        [Fact]
        public async Task GetAppointmentList_ShouldNotSelectDeletedAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId,
                    AppointmentStates = new[] { AppointmentState.Opened, AppointmentState.Cancelled },
                    DateRange = new Range<DateTime?>(new DateTime(2021, 11, 15), new DateTime(2021, 11, 20))
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            // Assert
            AssertResponse(response, Array.Empty<Guid>());
        }

        [Fact]
        public async Task GetAppointmentList_ShouldSelectAllNotDeletedAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = _doctorId
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            var expectedAppointmentIds = GetAppointments()
                .Where(appointment => appointment.Attendees.Any(_ => _.UserId == _doctorId) && !appointment.IsDeleted)
                .Select(_ => _.Id)
                .ToArray();

            // Assert
            AssertResponse(response, expectedAppointmentIds);
        }

        [Fact]
        public async Task GetAppointmentList_RandomAttendeeIdShouldNotSelectAnyAppointments()
        {
            // Arrange
            await DbContext.AddRangeAsync(GetAppointments());
            await DbContext.SaveChangesAsync();

            var requestDto = new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = Guid.NewGuid()
                },
                Paging = new PagingRequestDto()
            };

            // Act
            var response = await Service.GetAppointmentList(requestDto);

            // Assert
            AssertResponse(response, Array.Empty<Guid>());
        }

        private static void AssertResponse(AppointmentListResponseDto response, params Guid[] expectedIds)
        {
            Assert.NotNull(response);
            Assert.Equal(expectedIds.Length, response.Items.Count);
            var actualIds = response.Items.Select(x => x.Id).ToList();
            Assert.Empty(expectedIds.Except(actualIds));
        }

        private Appointment GetAppointment(Guid appointmentId, Guid attendeeId, DateTime startDateTime,
            AppointmentStatus status, bool isDeleted = false)
        {
            return new Fixture().Build<Appointment>()
                .With(e => e.Id, appointmentId)
                .With(e => e.Title, string.Empty)
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Duration, TimeSpan.FromHours(1))
                .With(e => e.Status, status)
                .With(e => e.IsDeleted, isDeleted)
                .With(e => e.Attendees, new List<AppointmentAttendee>()
                {
                    new Fixture().Build<AppointmentAttendee>()
                        .With(e => e.AppointmentId, appointmentId)
                        .With(e => e.UserId, attendeeId)
                        .With(e => e.IsDeleted, isDeleted)
                        .Create(),
                    new Fixture().Build<AppointmentAttendee>()
                        .With(e => e.AppointmentId, appointmentId)
                        .With(e => e.IsDeleted, isDeleted)
                        .Create()
                })
                .Create();
        }

        private List<Appointment> GetAppointments()
        {
            var startDateTime = DateTime.UtcNow.AddMinutes(-30);

            return new List<Appointment>
                {
                    // Already done appointments
                    GetAppointment(_appointmentIds[0], _doctorId, new DateTime(2021, 11, 09), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[1], _doctorId, new DateTime(2021, 11, 11), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[3], Guid.NewGuid(), new DateTime(2021, 11, 15), AppointmentStatus.Opened),
                    // Incorrect status
                    GetAppointment(_appointmentIds[2], _doctorId, new DateTime(2021, 11, 15), AppointmentStatus.Default),
                    // Deleted appointments
                    GetAppointment(_appointmentIds[4], _doctorId, new DateTime(2021, 11, 15), AppointmentStatus.Opened, isDeleted: true),
                    GetAppointment(_appointmentIds[5], _doctorId, new DateTime(2021, 11, 19), AppointmentStatus.Cancelled, isDeleted: true),
                    GetAppointment(_appointmentIds[6], _doctorId, startDateTime, AppointmentStatus.Opened, isDeleted: true),
                    // Currently ongoing appointments
                    GetAppointment(_appointmentIds[7], _doctorId, DateTime.UtcNow.AddMinutes(-24), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[8], Guid.NewGuid(), DateTime.UtcNow.AddMinutes(-30), AppointmentStatus.Opened),
                     // Currently open appointments
                    GetAppointment(_appointmentIds[9], _doctorId, DateTime.UtcNow.AddDays(1), AppointmentStatus.Opened),
                    GetAppointment(_appointmentIds[10], _doctorId, DateTime.UtcNow.AddMonths(1), AppointmentStatus.Opened),
                    // Cancelled appointments
                    GetAppointment(_appointmentIds[11], _doctorId, DateTime.UtcNow.AddDays(1), AppointmentStatus.Cancelled),
                    GetAppointment(_appointmentIds[12], Guid.NewGuid(), DateTime.UtcNow.AddDays(1), AppointmentStatus.Cancelled),
                    // Missed appointments
                    GetAppointment(_appointmentIds[13], _doctorId, DateTime.UtcNow.AddDays(-1), AppointmentStatus.Missed),
                    GetAppointment(_appointmentIds[14], _doctorId, DateTime.UtcNow.AddDays(-8), AppointmentStatus.Missed),
                    GetAppointment(_appointmentIds[15], Guid.NewGuid(), DateTime.UtcNow.AddMonths(-1), AppointmentStatus.Missed)
                };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
