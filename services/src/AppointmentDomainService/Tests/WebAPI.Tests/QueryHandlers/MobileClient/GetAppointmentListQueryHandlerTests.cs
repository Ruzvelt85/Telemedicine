using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using AutoMapper;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.WebAPI;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.MobileClient;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers.MobileClient
{
    public class GetAppointmentListQueryHandlerTests
    {
        private readonly AppointmentDomainServiceDbContext _context;
        private readonly GetAppointmentListQueryHandler _queryHandler;

        public GetAppointmentListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase($"MobileClient_GetAppointmentListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetAppointmentListQueryHandler(mapper, new AppointmentReadRepository(_context));
        }

        [Fact]
        public async Task EmptyRepository_ShouldReturnEmptyAppointmentList()
        {
            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task DefaultQuery_ShouldNotReturnAppointment()
        {
            var attendees = new List<AppointmentAttendee> { new Fixture().Build<AppointmentAttendee>().Create() };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, false).With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task OpenedAppointmentWithPatient_ShouldReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(patientId).Create() };
            var appointment = GetAppointmentComposer(AppointmentStatus.Opened, DateTime.UtcNow.AddDays(2))
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = patientId });

            Assert.NotNull(result);
            Assert.Single(result.Appointments);
        }

        [Fact]
        public async Task OngoingAppointmentWithPatient_ShouldReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(patientId).Create() };
            var appointment = GetAppointmentComposer(AppointmentStatus.Opened, DateTime.UtcNow.AddMinutes(-15), TimeSpan.FromHours(3))
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = patientId });

            Assert.NotNull(result);
            Assert.Single(result.Appointments);

            var appointmentResponse = result.Appointments.First();
            Assert.Equal(appointment.Id, appointmentResponse.Id);
            Assert.Equal(appointment.Title, appointmentResponse.Title);
            Assert.Equal(appointment.StartDate, appointmentResponse.StartDate);
            Assert.Equal(appointment.Duration, appointmentResponse.Duration);
            Assert.Equal(appointment.GetState().ToString(), appointmentResponse.State.ToString());
            Assert.Equal(appointment.Type.ToString(), appointmentResponse.Type.ToString());
            Assert.False(appointmentResponse.IsDeleted);

            Assert.Single(appointmentResponse.Attendees);
            Assert.Equal(attendees.First().UserId, appointmentResponse.Attendees.First());
        }


        [Fact]
        public async Task DoneAppointmentWithPatient_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(patientId).Create() };
            var appointment = GetAppointmentComposer(AppointmentStatus.Opened, DateTime.UtcNow.AddHours(-2), TimeSpan.FromHours(1))
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = patientId });

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task MissedAppointmentWithPatient_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(patientId).Create() };
            var appointment = GetAppointmentComposer(AppointmentStatus.Missed)
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = patientId });

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task CancelledAppointmentWithPatient_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(patientId).Create() };
            var appointment = GetAppointmentComposer(AppointmentStatus.Cancelled)
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = patientId });

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task DeletedAppointment_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(patientId).Create() };
            var appointment = GetAppointmentComposer(isDeleted: true).With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = patientId });

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task AppointmentWithoutAnyAttendee_ShouldNotReturnAppointment()
        {
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer(isDeleted: true).Create() };
            var appointment = GetAppointmentComposer().With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = Guid.NewGuid() });

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task AppointmentWithoutSameAttendee_ShouldNotReturnAppointment()
        {
            var attendees = new List<AppointmentAttendee> { GetAppointmentAttendeeComposer().Create() };
            var appointment = GetAppointmentComposer().With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetAppointmentListQuery { AttendeeId = Guid.NewGuid() });

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        private static IPostprocessComposer<Appointment> GetAppointmentComposer(AppointmentStatus? status = null,
            DateTime? startDate = null, TimeSpan? duration = null, bool isDeleted = false)
        {
            return new Fixture().Build<Appointment>()
                .With(_ => _.Status, status)
                .With(_ => _.StartDate, startDate)
                .With(_ => _.Duration, duration)
                .With(_ => _.IsDeleted, isDeleted);
        }

        private static IPostprocessComposer<AppointmentAttendee> GetAppointmentAttendeeComposer(Guid? patientId = null, bool isDeleted = false)
        {
            return new Fixture().Build<AppointmentAttendee>()
                .With(_ => _.UserId, patientId)
                .With(_ => _.IsDeleted, isDeleted);
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
