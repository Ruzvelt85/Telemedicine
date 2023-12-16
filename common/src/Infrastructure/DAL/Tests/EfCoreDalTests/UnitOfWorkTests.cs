using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Simple;
using Telemedicine.Common.Infrastructure.DomainEventInfrastructure;
using Telemedicine.Common.Infrastructure.EntityBase;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly TestEntityReadRepository _entityReadRepository;
        private readonly TestEntityWriteRepository _entityWriteRepository;
        private readonly TestSimpleEntityReadRepository _simpleEntityReadRepository;
        private readonly TestSimpleEntityWriteRepository _simpleEntityWriteRepository;
        private readonly UnitOfWork<TestDbContext> _unitOfWork;

        public UnitOfWorkTests()
        {
            // For every unit test we have to specify different name of InMemory database
            // in order to prevent unstable results of tests due to only one service-provider
            var dbContextOptions =
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase($"UnitOfWorkTests-{Guid.NewGuid()}");

            var domainEventPublisherMock = new Mock<IDomainEventPublisher>();
            domainEventPublisherMock
                .Setup(_ => _.PublishAsync(It.IsAny<DomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _context = new TestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            _context.Database.EnsureCreated();

            _unitOfWork = new UnitOfWork<TestDbContext>(_context, domainEventPublisherMock.Object);

            _entityReadRepository = new TestEntityReadRepository(_context);
            _entityWriteRepository = new TestEntityWriteRepository(_context);
            _simpleEntityReadRepository = new TestSimpleEntityReadRepository(_context);
            _simpleEntityWriteRepository = new TestSimpleEntityWriteRepository(_context);
        }

        [Fact]
        public async Task TestUnitOfWorkWithoutSaving()
        {
            var entities = await _entityReadRepository.GetAllAsync();
            var simpleEntities = await _simpleEntityReadRepository.GetAllAsync();

            Assert.Empty(entities);
            Assert.Empty(simpleEntities);

            var newEntity = GenerateTestEntity();
            await _entityWriteRepository.AddAsync(newEntity);

            var newSimpleEntity1 = GenerateTestSimpleEntity();
            var newSimpleEntity2 = GenerateTestSimpleEntity();
            await _simpleEntityWriteRepository.AddRangeAsync(new[] { newSimpleEntity1, newSimpleEntity2 });

            entities = await _entityReadRepository.GetAllAsync();
            simpleEntities = await _simpleEntityReadRepository.GetAllAsync();

            Assert.Empty(entities);
            Assert.Empty(simpleEntities);

            _context.Entities.Remove(newEntity);
            _context.SimpleEntities.RemoveRange(newSimpleEntity1, newSimpleEntity2);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task TestUnitOfWorkWithSaving()
        {
            var entities = await _entityReadRepository.GetAllAsync();
            var simpleEntities = await _simpleEntityReadRepository.GetAllAsync();

            Assert.Empty(entities);
            Assert.Empty(simpleEntities);

            var newEntity = GenerateTestEntity();
            await _entityWriteRepository.AddAsync(newEntity);

            var newSimpleEntity1 = GenerateTestSimpleEntity();
            var newSimpleEntity2 = GenerateTestSimpleEntity();
            await _simpleEntityWriteRepository.AddRangeAsync(new[] { newSimpleEntity1, newSimpleEntity2 });
            await _unitOfWork.SaveAsync();

            entities = await _entityReadRepository.GetAllAsync();
            simpleEntities = await _simpleEntityReadRepository.GetAllAsync();

            Assert.Single(entities);
            Assert.Equal(2, simpleEntities.Count);

            await _entityWriteRepository.DeleteAsync(newEntity);
            await _simpleEntityWriteRepository.DeleteRangeAsync(new[] { newSimpleEntity1, newSimpleEntity2 });
            await _unitOfWork.SaveAsync();
        }

        private TestEntity GenerateTestEntity()
        {
            var fakeEntity = new Faker<TestEntity>()
                .RuleFor(p => p.Id, _ => Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Name.FullName())
                .RuleFor(p => p.Age, f => f.Person.Random.Byte(75, 95))
                .RuleFor(p => p.BirthDate, f => f.Person.DateOfBirth)
                .RuleFor(p => p.TimeSinceLastCall, f => (DateTime.Now - f.Date.Recent(2)).Duration());

            return fakeEntity.Generate();
        }

        private TestSimpleEntity GenerateTestSimpleEntity()
        {
            return new TestSimpleEntity(Guid.NewGuid());
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
