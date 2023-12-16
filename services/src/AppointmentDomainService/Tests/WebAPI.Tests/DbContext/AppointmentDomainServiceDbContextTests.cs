using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.DbContext
{
    public class AppointmentDomainServiceDbContextTests
    {
        private readonly AppointmentDomainServiceDbContext _context;

        public AppointmentDomainServiceDbContextTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase($"AppointmentDomainServiceDbContextTests-{Guid.NewGuid()}");

            _context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task AddAppointmentWithAttendee_ShouldReturnAppointmentAndAttendee()
        {
            var attendees = new List<AppointmentAttendee> { new Fixture().Build<AppointmentAttendee>().Create() };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            Assert.Single(_context.Appointments.IgnoreQueryFilters());
            Assert.Single(_context.Appointments.IgnoreQueryFilters().First().Attendees);
        }

        [Fact]
        public async Task DeleteAppointmentWithAttendee_ShouldReturnAppointmentAndAttendeeWithIsDeleted()
        {
            var attendees = new List<AppointmentAttendee> { new Fixture().Build<AppointmentAttendee>().Create() };
            var appointment = new Fixture().Build<Appointment>().With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();
            _context.Remove(appointment);
            await _context.SaveChangesAsync();

            var appointments = _context.Appointments.IgnoreQueryFilters();
            Assert.Single(appointments);

            var firstAppointment = appointments.First();
            Assert.Single(firstAppointment.Attendees);
            Assert.True(firstAppointment.IsDeleted);
            Assert.NotEqual(firstAppointment.CreatedOn, firstAppointment.UpdatedOn);

            var firstAttendee = firstAppointment.Attendees.First();
            Assert.True(firstAttendee.IsDeleted);
        }

        [Fact]
        public async Task ModifyAppointment_ShouldChangeUpdatedOnProperty()
        {
            var appointment = new Fixture().Build<Appointment>().Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            Assert.Single(_context.Appointments.IgnoreQueryFilters());
            var firstAppointment = _context.Appointments.IgnoreQueryFilters().First();
            Assert.NotEqual(firstAppointment.CreatedOn, firstAppointment.UpdatedOn);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());
            return mockOptions;
        }
    }
}
