using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Refit;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class AppointmentCommandServiceTests : IDisposable, IHttpServiceTests<IAppointmentCommandService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        /// <inheritdoc />
        public HttpServiceFixture<IAppointmentCommandService> HttpServiceFixture { get; }

        /// <inheritdoc />
        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        public IAppointmentCommandService Service { get; }

        public AppointmentDomainServiceDbContext DbContext { get; }

        public AppointmentCommandServiceTests(
            HttpServiceFixture<IAppointmentCommandService> httpServiceFixture,
            EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            Service = HttpServiceFixture.GetRestService();
            DbContext = EfCoreContextFixture.DbContext;
        }

        #region UpdateStatus

        [Fact]
        public async Task UpdateStatus_DefaultRequest_ShouldThrowValidationApiException()
        {
            var request = new UpdateAppointmentStatusRequestDto();
            var exception = await Record.ExceptionAsync(() => Service.UpdateStatus(request));

            Assert.NotNull(exception);
            Assert.IsType<ValidationApiException>(exception);
        }

        [Fact]
        public async Task UpdateStatus_NoAppointment_ShouldThrowApiException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var request = new UpdateAppointmentStatusRequestDto() { Id = appointmentId, Status = AppointmentStatus.Cancelled, Reason = "Qwerty" };

            // Act
            var exception = await Record.ExceptionAsync(() => Service.UpdateStatus(request));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        [Fact]
        public async Task UpdateStatus_ShouldUpdateOnlyAppointmentWithSpecificId()
        {
            // Arrange
            var newAppointment1 = GetAppointment(Guid.NewGuid(), DateTime.UtcNow.AddDays(1), Core.Enums.AppointmentStatus.Opened, false);
            var newAppointment2 = GetAppointment(Guid.NewGuid(), DateTime.UtcNow.AddDays(5), Core.Enums.AppointmentStatus.Cancelled, false);
            await DbContext.AddRangeAsync(newAppointment1, newAppointment2);
            await DbContext.SaveChangesAsync();

            const string cancelReason = "Qwerty";
            var request = new UpdateAppointmentStatusRequestDto { Id = newAppointment1.Id, Reason = cancelReason, Status = AppointmentStatus.Cancelled };

            // Act
            await Service.UpdateStatus(request);
            await DbContext.SaveChangesAsync();

            // Assert
            var appointment1 = await DbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == newAppointment1.Id);
            Assert.NotNull(appointment1);
            Assert.Equal(Core.Enums.AppointmentStatus.Cancelled, appointment1.Status);
            Assert.Equal(cancelReason, appointment1.CancelReason);

            var appointment2 = await DbContext.Appointments.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == newAppointment2.Id);
            Assert.NotNull(appointment2);
            Assert.Equal(newAppointment2.Status, appointment2.Status);
            Assert.Equal(newAppointment2.CancelReason, appointment2.CancelReason);
        }

        #endregion

        #region CreateAppointment

        [Fact]
        public async Task CreateAppointment_EmptyRepository_ShouldCreateAppointmentAndReturnId()
        {
            var request = new CreateAppointmentRequestDto
            {
                Title = "Title",
                Description = "Description",
                AppointmentType = AppointmentType.Urgent,
                CreatorId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddHours(1),
                Duration = TimeSpan.FromHours(1),
                AttendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
            };
            var newAppointmentId = await Service.CreateAppointment(request);

            await DbContext.SaveChangesAsync();

            // Assert
            var appointment = await DbContext.Appointments.Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == newAppointmentId);
            Assert.NotNull(appointment);
            Assert.Equal(request.Title, appointment.Title);
            Assert.Equal(request.Description, appointment.Description);
            Assert.Equal(request.Duration, appointment.Duration);
            Assert.Equal((int)request.AppointmentType, (int)appointment.Type);
            Assert.Equal(request.CreatorId, appointment.CreatorId);
            Assert.Equal(request.AttendeeIds.Length, appointment.Attendees.Count);
            Assert.Contains(appointment.Attendees, _ => _.UserId == request.AttendeeIds[0]);
            Assert.Contains(appointment.Attendees, _ => _.UserId == request.AttendeeIds[1]);
        }

        [Fact]
        public async Task CreateAppointment_WithSameAttendeeIds_ShouldThrowValidationApiException()
        {
            var attendeeId = Guid.NewGuid();
            var request = new CreateAppointmentRequestDto
            {
                Title = "Title",
                Description = "Description",
                AppointmentType = AppointmentType.Urgent,
                CreatorId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow.AddHours(1),
                Duration = TimeSpan.FromHours(1),
                AttendeeIds = new[] { attendeeId, attendeeId }
            };

            var exception = await Record.ExceptionAsync(() => Service.CreateAppointment(request));

            Assert.NotNull(exception);
            Assert.IsType<ValidationApiException>(exception);
        }

        [Fact]
        public async Task CreateAppointment_InvalidRequest_ShouldThrowValidationApiException()
        {
            var request = new CreateAppointmentRequestDto();

            var exception = await Record.ExceptionAsync(() => Service.CreateAppointment(request));

            Assert.NotNull(exception);
            Assert.IsType<ValidationApiException>(exception);
        }

        [Fact]
        public async Task CreateAppointment_WithOverlappedAppointment_ShouldThrowApiException()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddHours(1);
            var duration = TimeSpan.FromHours(1);
            var attendeeId = Guid.NewGuid();
            var initAppointmentId = Guid.NewGuid();
            var newAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, initAppointmentId)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.Status, Core.Enums.AppointmentStatus.Opened)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.Attendees, new List<AppointmentAttendee>()
                {
                    new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, attendeeId).With(_ => _.IsDeleted, false).Create(),
                    new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, Guid.NewGuid).With(_ => _.IsDeleted, false).Create(),
                })
                .Create();
            await DbContext.AddAsync(newAppointment);
            await DbContext.SaveChangesAsync();

            var request = new CreateAppointmentRequestDto()
            {
                Title = "Title",
                Description = "Description",
                AppointmentType = AppointmentType.Urgent,
                CreatorId = Guid.NewGuid(),
                StartDate = startDate,
                Duration = duration,
                AttendeeIds = new[] { attendeeId, Guid.NewGuid() }
            };

            var exception = await Record.ExceptionAsync(() => Service.CreateAppointment(request));

            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        #endregion

        #region RescheduleAppointment

        [Fact]
        public async Task RescheduleAppointment_NoOverlapped_ShouldRescheduleAppointmentAndReturnNewId()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.IsDeleted, false)
                .With(_ => _.CancelReason, (string?)null)
                .With(_ => _.Attendees, attendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            DbContext.Appointments.Add(appointment);
            await DbContext.SaveChangesAsync();

            var originalAppBeforeReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);

            var request = new Fixture().Build<RescheduleAppointmentRequestDto>()
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1).Date)
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.Reason, "Test reason")
                .Create();

            //Act
            Guid newAppointmentId = await Service.Reschedule(appointmentId, request);

            //Assert
            var originalAppAfterReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);
            originalAppAfterReschedule.Should().NotBeNull();
            originalAppAfterReschedule!.GetState().Should().Be(Core.Enums.AppointmentState.Cancelled);
            originalAppAfterReschedule.CancelReason.Should().Be(request.Reason);
            Assert.NotEqual(originalAppBeforeReschedule, originalAppAfterReschedule);
            originalAppAfterReschedule.Should().BeEquivalentTo(originalAppBeforeReschedule,
                opt => opt.Excluding(a => a.Status)
                                                                 .Excluding(a => a.UpdatedOn)
                                                                 .Excluding(a => a.CreatedOn)
                                                                 .Excluding(a => a.CancelReason));

            var newAppointment = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == newAppointmentId);
            newAppointment.Should().NotBeNull();
            newAppointment.Id.Should().NotBe(originalAppAfterReschedule.Id);
            newAppointment!.GetState().Should().Be(Core.Enums.AppointmentState.Opened);
            newAppointment.Title.Should().Be(originalAppBeforeReschedule.Title);
            newAppointment.StartDate.Should().Be(request.StartDate);
            newAppointment.Duration.Should().Be(request.Duration);
            newAppointment.CancelReason.Should().BeNull();
            newAppointment.CreatorId.Should().Be(request.CreatorId);
            newAppointment.IsDeleted.Should().Be(false);
            newAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            newAppointment.Attendees.Select(a => a.UserId).Should().BeEquivalentTo(attendeeIds);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenOverlappedWithItself_ShouldRescheduleAppointmentAndReturnNewId()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            DateTime startDate = DateTime.UtcNow.AddDays(1).Date;
            TimeSpan duration = TimeSpan.FromHours(1);
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.Status, Core.Enums.AppointmentStatus.Opened)
                .With(_ => _.Attendees, attendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            var otherNotOverlappedAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, Guid.NewGuid())
                .With(_ => _.StartDate, DateTime.UtcNow.AddHours(1))
                .With(_ => _.Duration, TimeSpan.FromMinutes(15))
                .With(_ => _.Status, Core.Enums.AppointmentStatus.Opened)
                .With(_ => _.Attendees, attendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            DbContext.Appointments.Add(appointment);
            DbContext.Appointments.Add(otherNotOverlappedAppointment);
            await DbContext.SaveChangesAsync();

            var originalAppBeforeReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);

            var request = new Fixture().Build<RescheduleAppointmentRequestDto>()
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.Reason, "Test reason")
                .Create();

            //Act
            Guid newAppointmentId = await Service.Reschedule(appointmentId, request);

            //Assert
            var originalAppAfterReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);
            originalAppAfterReschedule.Should().NotBeNull();
            originalAppAfterReschedule!.GetState().Should().Be(Core.Enums.AppointmentState.Cancelled);
            originalAppAfterReschedule.CancelReason.Should().Be(request.Reason);
            Assert.NotEqual(originalAppBeforeReschedule, originalAppAfterReschedule);
            originalAppAfterReschedule.Should().BeEquivalentTo(originalAppBeforeReschedule,
                opt => opt.Excluding(a => a.Status)
                        .Excluding(a => a.UpdatedOn)
                        .Excluding(a => a.CreatedOn)
                        .Excluding(a => a.CancelReason));

            var newAppointment = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == newAppointmentId);
            newAppointment.Should().NotBeNull();
            newAppointment.Id.Should().NotBe(originalAppAfterReschedule.Id);
            newAppointment!.GetState().Should().Be(Core.Enums.AppointmentState.Opened);
            newAppointment.Title.Should().Be(originalAppBeforeReschedule.Title);
            newAppointment.StartDate.Should().Be(request.StartDate);
            newAppointment.Duration.Should().Be(request.Duration);
            newAppointment.CancelReason.Should().BeNull();
            newAppointment.CreatorId.Should().Be(request.CreatorId);
            newAppointment.IsDeleted.Should().Be(false);
            newAppointment.Attendees.Should().HaveCount(attendeeIds.Length);
            newAppointment.Attendees.Select(a => a.UserId).Should().BeEquivalentTo(attendeeIds);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenOverlapped_ShouldThrowOverlappedException()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            DateTime startDate = DateTime.UtcNow.AddDays(1);
            TimeSpan duration = TimeSpan.FromHours(1);
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddHours(1))
                .With(_ => _.Status, Core.Enums.AppointmentStatus.Opened)
                .With(_ => _.Attendees, attendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            var overlappedAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, Guid.NewGuid())
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.Status, Core.Enums.AppointmentStatus.Opened)
                .With(_ => _.Attendees, attendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            DbContext.Appointments.Add(appointment);
            DbContext.Appointments.Add(overlappedAppointment);
            await DbContext.SaveChangesAsync();

            var originalAppBeforeReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);

            var request = new Fixture().Build<RescheduleAppointmentRequestDto>()
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.Reason, "Test reason")
                .Create();

            //Act
            var ex = await Record.ExceptionAsync(() => Service.Reschedule(appointmentId, request));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ApiException>(ex); //We are not actually checking for the expected exception here!!

            var originalAppAfterReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);
            originalAppAfterReschedule.Should().NotBeNull();
            Assert.NotEqual(originalAppBeforeReschedule, originalAppAfterReschedule);
            originalAppAfterReschedule.Should().BeEquivalentTo(originalAppBeforeReschedule);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenIncorrectStatus_ShouldThrowApiException()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            var attendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddHours(-1))
                .With(_ => _.Status, Core.Enums.AppointmentStatus.Cancelled)
                .With(_ => _.Attendees, attendeeIds.Select(id => new AppointmentAttendee(id)).ToArray())
                .Create();

            DbContext.Appointments.Add(appointment);
            await DbContext.SaveChangesAsync();

            var originalAppBeforeReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);

            var request = new Fixture().Build<RescheduleAppointmentRequestDto>()
                .With(_ => _.StartDate, DateTime.UtcNow.AddHours(1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.Reason, "Test reason")
                .Create();

            //Act
            var ex = await Record.ExceptionAsync(() => Service.Reschedule(appointmentId, request));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ApiException>(ex); //We are not actually checking for the expected exception here!!

            var originalAppAfterReschedule = await DbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == appointmentId);
            originalAppAfterReschedule.Should().NotBeNull();
            Assert.NotEqual(originalAppBeforeReschedule, originalAppAfterReschedule);
            originalAppAfterReschedule.Should().BeEquivalentTo(originalAppBeforeReschedule);
        }

        [Fact]
        public async Task RescheduleAppointment_WhenNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            var request = new Fixture().Build<RescheduleAppointmentRequestDto>()
                .With(_ => _.StartDate, DateTime.UtcNow.AddHours(1))
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.Reason, "Test reason")
                .Create();

            //Act
            var ex = await Record.ExceptionAsync(() => Service.Reschedule(appointmentId, request));

            //Assert
            Assert.NotNull(ex);
            Assert.IsType<ApiException>(ex); //We are not actually checking for the expected exception here!!
        }

        #endregion

        private static Appointment GetAppointment(Guid appointmentId, DateTime startDate, Core.Enums.AppointmentStatus status, bool isDeleted)
        {
            return new Fixture().Build<Appointment>()
                .With(_ => _.StartDate, DateTime.UtcNow.AddMonths(5))
                .With(_ => _.Id, appointmentId)
                .With(_ => _.Status, status)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, TimeSpan.FromHours(1))
                .With(_ => _.IsDeleted, isDeleted)
                .Create();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
