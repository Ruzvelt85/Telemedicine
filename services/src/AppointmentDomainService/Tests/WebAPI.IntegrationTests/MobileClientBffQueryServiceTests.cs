using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class MobileClientBffQueryServiceTests
        : IDisposable, IHttpServiceTests<IMobileClientBffQueryService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        /// <inheritdoc />
        public HttpServiceFixture<IMobileClientBffQueryService> HttpServiceFixture { get; }

        /// <inheritdoc />
        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        public IMobileClientBffQueryService Service { get; }

        public AppointmentDomainServiceDbContext DbContext { get; }

        public MobileClientBffQueryServiceTests(
            HttpServiceFixture<IMobileClientBffQueryService> httpServiceFixture,
            EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            Service = HttpServiceFixture.GetRestService();
            DbContext = EfCoreContextFixture.DbContext;
        }

        [Fact]
        public async Task GetAppointmentList_EmptyRepository_ShouldReturnEmptyAppointmentList()
        {
            var response = await Service.GetAppointmentList(new AppointmentListRequestDto() { AttendeeId = Guid.NewGuid() });

            Assert.NotNull(response);
            Assert.Empty(response.Appointments);
        }

        [Fact]
        public async Task GetAppointmentList_DefaultRequest_ShouldReturnOnlyNotDeletedOpenedWithSpecialAttendee()
        {
            var (openedAppointmentId, ongoingAppointmentId, deletedAppointmentId, patientId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var deletedAppointment = GetAppointment(deletedAppointmentId, patientId);
            var appointments = new List<Appointment>()
            {
                GetAppointment(openedAppointmentId, patientId, AppointmentStatus.Opened, DateTime.UtcNow.AddDays(2)),
                GetAppointment(ongoingAppointmentId, patientId, AppointmentStatus.Opened, DateTime.UtcNow.AddHours(-1), TimeSpan.FromHours(3)),
                GetAppointment(Guid.NewGuid(), patientId, AppointmentStatus.Missed),
                GetAppointment(Guid.NewGuid(), patientId, AppointmentStatus.Cancelled),
                GetAppointment(Guid.NewGuid(), patientId, isDeleted: true),
                GetAppointment(Guid.NewGuid(), Guid.NewGuid()),
                deletedAppointment
            };
            await DbContext.AddRangeAsync(appointments);
            await DbContext.SaveChangesAsync();
            DbContext.Remove(deletedAppointment);
            await DbContext.SaveChangesAsync();

            var response = await Service.GetAppointmentList(new AppointmentListRequestDto { AttendeeId = patientId });

            Assert.NotNull(response);
            Assert.Equal(2, response.Appointments.Count);
            Assert.Contains(response.Appointments, a => a.Id == openedAppointmentId);
            Assert.Contains(response.Appointments, a => a.Id == ongoingAppointmentId);
        }

        [Fact]
        public async Task GetChangedAppointmentList_EmptyRepository_ShouldReturnEmptyAppointmentList()
        {
            var response = await Service.GetChangedAppointmentList(new ChangedAppointmentListRequestDto(Guid.NewGuid(), DateTime.UtcNow));

            Assert.NotNull(response);
            Assert.Empty(response.Appointments);
        }

        [Fact]
        public async Task GetChangedAppointmentList_ShouldReturnModifiedDeletedAppointmentsWithSpecificPatient()
        {
            var (deletedAppointmentId, modifiedAppointmentId, appointmentId3, patientId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var deletedAppointment = GetAppointment(deletedAppointmentId, patientId);
            var modifiedAppointment = GetAppointment(modifiedAppointmentId, patientId);
            var appointmentWithRandomPatient = GetAppointment(appointmentId3, Guid.NewGuid());
            await DbContext.AddRangeAsync(deletedAppointment, modifiedAppointment, appointmentWithRandomPatient);
            await DbContext.SaveChangesAsync();
            DbContext.Update(modifiedAppointment);
            await DbContext.SaveChangesAsync();
            DbContext.Remove(deletedAppointment);
            await DbContext.SaveChangesAsync();

            var appointment = DbContext.Appointments.First(_ => _.Id == modifiedAppointmentId);
            var query = new ChangedAppointmentListRequestDto(patientId, appointment.CreatedOn);

            var response = await Service.GetChangedAppointmentList(query);

            Assert.NotNull(response);
            Assert.Equal(2, response.Appointments.Count);
            Assert.Contains(response.Appointments, a => a.Id == deletedAppointmentId);
            Assert.Contains(response.Appointments, a => a.Id == modifiedAppointmentId);
        }

        private static Appointment GetAppointment(Guid appointmentId, Guid patientId,
            AppointmentStatus? status = null, DateTime? startDate = null, TimeSpan? duration = null, bool isDeleted = false)
        {
            return new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.Status, status)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.IsDeleted, isDeleted)
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
