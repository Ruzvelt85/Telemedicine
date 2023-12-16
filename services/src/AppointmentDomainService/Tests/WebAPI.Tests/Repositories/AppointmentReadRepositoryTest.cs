using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.Repositories
{
    public class AppointmentReadRepositoryTest
    {
        private readonly AppointmentDomainServiceDbContext _context;

        private readonly AppointmentReadRepository _readRepository;

        public AppointmentReadRepositoryTest()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase($"AppointmentDomainServiceDbContextTests-{Guid.NewGuid()}");

            _context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            _readRepository = new AppointmentReadRepository(_context);
        }

        [Fact]
        public async void FindWithDeletedTest()
        {
            var threshold = DateTime.Now;
            var entities = _readRepository.FindWithDeleted(x => x.StartDate > threshold);

            Assert.Empty(entities.ToList());

            var deletedEntity = new Fixture().Build<Appointment>()
                .With(a => a.IsDeleted, true)
                .With(a => a.StartDate, threshold.AddDays(1))
                .Create();

            var notDeletedEntity = new Fixture().Build<Appointment>()
                .With(a => a.IsDeleted, false)
                .With(a => a.StartDate, threshold.AddDays(2))
                .Create();

            await _context.Appointments.AddRangeAsync(deletedEntity, notDeletedEntity);
            await _context.SaveChangesAsync();

            entities = _readRepository.FindWithDeleted(x => x.StartDate > threshold);

            Assert.Equal(2, entities.ToList().Count);

            _context.Appointments.Remove(notDeletedEntity);
            await _context.SaveChangesAsync();

            entities = _readRepository.FindWithDeleted(x => x.StartDate > threshold);

            Assert.Equal(2, entities.ToList().Count);
        }

        [Fact]
        public async Task FindTest()
        {
            var threshold = DateTime.Now;
            var entities = _readRepository.Find(x => x.StartDate > threshold);

            Assert.Empty(entities.ToList());

            var deletedEntity = new Fixture().Build<Appointment>()
                .With(a => a.IsDeleted, true)
                .With(a => a.StartDate, threshold.AddDays(1))
                .Create();

            var notDeletedEntity = new Fixture().Build<Appointment>()
                .With(a => a.IsDeleted, false)
                .With(a => a.StartDate, threshold.AddDays(2))
                .Create();

            await _context.Appointments.AddRangeAsync(deletedEntity, notDeletedEntity);
            await _context.SaveChangesAsync();

            entities = _readRepository.Find(x => x.StartDate > threshold);

            Assert.Single(entities.ToList());

            _context.Appointments.Remove(notDeletedEntity);
            await _context.SaveChangesAsync();

            entities = _readRepository.Find(x => x.StartDate > threshold);

            Assert.Empty(entities.ToList());
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());
            return mockOptions;
        }
    }
}
