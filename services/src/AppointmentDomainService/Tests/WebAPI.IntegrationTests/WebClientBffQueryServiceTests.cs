using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using AppointmentType = Telemedicine.Services.AppointmentDomainService.API.Common.Common.AppointmentType;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class WebClientBffQueryServiceTests
        : IDisposable, IHttpServiceTests<IWebClientBffQueryService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        public HttpServiceFixture<IWebClientBffQueryService> HttpServiceFixture { get; }

        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        public AppointmentDomainServiceDbContext DbContext { get; }
        public IWebClientBffQueryService Service { get; }

        public WebClientBffQueryServiceTests(
            HttpServiceFixture<IWebClientBffQueryService> httpServiceFixture,
            EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetNearestAppointmentsByAttendeeId_IntegrationTest_ExistManyAppointments()
        {
            var (patientId, previousAppointmentId, nextAppointmentId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var appointments = new List<Appointment>
            {
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-2), patientId, Core.Enums.AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(1), patientId, Core.Enums.AppointmentType.Urgent, AppointmentStatus.Opened),
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), patientId, Core.Enums.AppointmentType.SemiAnnual, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddHours(-8), Guid.NewGuid(), Core.Enums.AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddHours(-5), patientId, Core.Enums.AppointmentType.Annual, AppointmentStatus.Opened, true),
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddHours(5), patientId, Core.Enums.AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddHours(2), patientId, Core.Enums.AppointmentType.Urgent, AppointmentStatus.Opened, true),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-3), patientId, Core.Enums.AppointmentType.Urgent, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddHours(3), Guid.NewGuid(), Core.Enums.AppointmentType.Annual, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(200), patientId)
            };
            await DbContext.AddRangeAsync(appointments);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetNearestAppointmentsByAttendeeIdAsync(patientId);

            Assert.NotNull(response.PreviousAppointmentInfo);
            Assert.Equal(previousAppointmentId, response.PreviousAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentInfo);
            Assert.Equal(nextAppointmentId, response.NextAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentType);
            Assert.Equal(AppointmentType.FollowUp, response.NextAppointmentType!.Value);
        }

        [Fact]
        public async Task GetNearestAppointmentsByAttendeeId_IntegrationTest_NotExistAppointmentsForSpecifiedPatient()
        {
            var (patientId, previousAppointmentId, nextAppointmentId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var appointments = new List<Appointment>
            {
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-2), Guid.NewGuid(), Core.Enums.AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(1), Guid.NewGuid(), Core.Enums.AppointmentType.Urgent, AppointmentStatus.Opened),
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), Guid.NewGuid(), Core.Enums.AppointmentType.SemiAnnual, AppointmentStatus.Opened),
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddHours(5), Guid.NewGuid(), Core.Enums.AppointmentType.Annual, AppointmentStatus.Opened)
            };
            await DbContext.AddRangeAsync(appointments);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetNearestAppointmentsByAttendeeIdAsync(patientId);

            Assert.Null(response.PreviousAppointmentInfo);
            Assert.Null(response.NextAppointmentInfo);
            Assert.Null(response.NextAppointmentType);
        }

        [Fact]
        public async Task GetAppointmentList_IntegrationTest_PagingTest()
        {
            var (doctorId, appointmentId) = (Guid.NewGuid(), Guid.NewGuid());
            var dateTimeUtcNow = DateTime.UtcNow;
            var appointments = new List<Appointment>
            {
                GetAppointment(Guid.NewGuid(), "Some Appointment", dateTimeUtcNow.AddDays(1), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Some Appointment", dateTimeUtcNow.AddDays(2), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(appointmentId, "Appointment 1 match by date and paging", dateTimeUtcNow.AddDays(4), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Appointment 2 match by date", dateTimeUtcNow.AddDays(5), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Appointment 3 match by date", dateTimeUtcNow.AddDays(6), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Appointment 4 match by date", dateTimeUtcNow.AddDays(7), doctorId, status: AppointmentStatus.Opened)
            };
            await DbContext.AddRangeAsync(appointments);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetAppointmentList(new AppointmentListRequestDto
            {
                Filter = new AppointmentListFilterRequestDto
                {
                    AttendeeId = doctorId,
                    AppointmentStates = new[] { API.Common.Common.AppointmentState.Opened },
                    DateRange = new Range<DateTime?>(dateTimeUtcNow.AddDays(4), dateTimeUtcNow.AddDays(7))
                },
                Paging = new PagingRequestDto(1, 2)
            });

            Assert.NotNull(response);
            Assert.NotEmpty(response.Items);
            Assert.Single(response.Items);
            Assert.Equal(3, response.Paging.Total);
            Assert.Equal(appointmentId, response.Items.First().Id);
        }

        private static Appointment GetAppointment(Guid appointmentId, string appointmentName,
            DateTime startDateTime, Guid attendeeId, Core.Enums.AppointmentType type = Core.Enums.AppointmentType.Default,
            AppointmentStatus status = AppointmentStatus.Default, bool isDeleted = false)
        {
            return new Fixture().Build<Appointment>()
                .With(e => e.Id, appointmentId)
                .With(e => e.Title, appointmentName)
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Duration, TimeSpan.FromHours(1))
                .With(e => e.Type, type)
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

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
