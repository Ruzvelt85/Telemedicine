using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
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
    public class GetChangedAppointmentListQueryHandlerTests
    {
        private readonly AppointmentDomainServiceDbContext _context;
        private readonly GetChangedAppointmentListQueryHandler _queryHandler;

        public GetChangedAppointmentListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase($"MobileClient_GetAppointmentListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetChangedAppointmentListQueryHandler(mapper, new AppointmentReadRepository(_context));
        }

        [Fact]
        public async Task EmptyRepository_ShouldReturnEmptyAppointmentList()
        {
            var result = await _queryHandler.HandleAsync(new GetChangedAppointmentListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task DefaultQuery_ShouldNotReturnAppointment()
        {
            var attendees = new List<AppointmentAttendee>()
            {
                new Fixture().Build<AppointmentAttendee>().Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, false).With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetChangedAppointmentListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task AppointmentWithPatientAndSameLastUpdate_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>()
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).With(_ => _.IsDeleted, false).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, false).With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();
            var createdDate = _context.Appointments.First().CreatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = createdDate };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task AppointmentWithLastUpdateLaterThanUpdatedOn_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>()
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, false).With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();
            var updatedOn = _context.Appointments.First().UpdatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = updatedOn.AddHours(1) };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task AppointmentWithPatientAndLastUpdateEarlierThanUpdatedOn_ShouldReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>()
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).With(_ => _.IsDeleted, false).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, false).With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();
            var updatedOn = _context.Appointments.First().UpdatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = updatedOn.AddHours(-1) };

            var result = await _queryHandler.HandleAsync(query);

            AssertAppointmentWithAttendee(result, appointment, attendees);
        }

        [Fact]
        public async Task DeletedAppointment_AfterLastUpdate_ShouldReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, true)
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var updatedOn = _context.Appointments.IgnoreQueryFilters().First().UpdatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = updatedOn.AddSeconds(-1) };

            var result = await _queryHandler.HandleAsync(query);

            AssertAppointmentWithAttendee(result, appointment, attendees);
        }


        [Fact]
        public async Task DeletedAppointment_SameLastUpdate_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, true)
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var updatedOn = _context.Appointments.IgnoreQueryFilters().First().UpdatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = updatedOn };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task DeletedAppointment_BeforeLastUpdate_ShouldNotReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, true).With(_ => _.Attendees, attendees).Create();
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var updatedOn = _context.Appointments.IgnoreQueryFilters().First().UpdatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = updatedOn.AddMinutes(10) };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task ModifiedAppointment_ShouldReturnAppointment()
        {
            var patientId = Guid.NewGuid();
            var attendees = new List<AppointmentAttendee>
            {
                new Fixture().Build<AppointmentAttendee>().With(_ => _.UserId, patientId).Create()
            };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.IsDeleted, false).With(_ => _.Attendees, attendees).Create();
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();
            _context.Update(appointment);
            await _context.SaveChangesAsync();
            var updatedOn = _context.Appointments.First().UpdatedOn;
            var query = new GetChangedAppointmentListQuery { AttendeeId = patientId, LastUpdate = updatedOn.AddMinutes(-10) };

            var result = await _queryHandler.HandleAsync(query);

            AssertAppointmentWithAttendee(result, appointment, attendees);
        }

        private static void AssertAppointmentWithAttendee(AppointmentListResponseDto actualAppointment, Appointment expectedAppointment, IEnumerable<AppointmentAttendee> expectedAttendees)
        {
            Assert.NotNull(actualAppointment);
            Assert.Single(actualAppointment.Appointments);

            var appointmentResponse = actualAppointment.Appointments.First();
            Assert.Equal(expectedAppointment.Id, appointmentResponse.Id);
            Assert.Equal(expectedAppointment.Title, appointmentResponse.Title);
            Assert.Equal(expectedAppointment.StartDate, appointmentResponse.StartDate);
            Assert.Equal(expectedAppointment.Duration, appointmentResponse.Duration);
            Assert.Equal(expectedAppointment.GetState().ToString(), appointmentResponse.State.ToString());
            Assert.Equal(expectedAppointment.Type.ToString(), appointmentResponse.Type.ToString());
            Assert.Equal(expectedAppointment.IsDeleted, appointmentResponse.IsDeleted);

            Assert.Single(appointmentResponse.Attendees);
            Assert.Equal(expectedAttendees.First().UserId, appointmentResponse.Attendees.First());
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
