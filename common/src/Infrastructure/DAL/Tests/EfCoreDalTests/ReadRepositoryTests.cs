using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests
{
    public class ReadRepositoryTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly TestEntityReadRepository _entityReadRepository;

        public ReadRepositoryTests()
        {
            // For every unit test we have to specify different name of InMemory database
            // in order to prevent unstable results of tests due to only one service-provider
            var dbContextOptions =
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase($"RepositoriesTests-{Guid.NewGuid()}");

            _context = new TestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            _context.Database.EnsureCreated();

            _entityReadRepository = new TestEntityReadRepository(_context);
        }

        [Fact]
        public async Task TestRepositoryGetAllEmpty()
        {
            var entities = await _entityReadRepository.GetAllAsync();

            Assert.Empty(entities);
        }

        [Fact]
        public async Task TestRepositoryGetAll()
        {
            var newEntity = GenerateTestEntity();

            await _context.Entities.AddAsync(newEntity);
            await _context.SaveChangesAsync();

            var entities = await _entityReadRepository.GetAllAsync();

            Assert.Single(entities);

            _context.Entities.Remove(newEntity);
            await _context.SaveChangesAsync();

            entities = await _entityReadRepository.GetAllAsync();

            Assert.Empty(entities);
        }

        [Fact]
        public async Task TestRepositoryGetQuery()
        {
            var dbEntities = _entityReadRepository.GetQuery();

            Assert.Empty(dbEntities);

            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            var entity3 = GenerateTestEntity();

            await _context.Entities.AddRangeAsync(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            dbEntities = _entityReadRepository.GetQuery();

            Assert.Equal(3, dbEntities.Count());

            _context.Entities.RemoveRange(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            dbEntities = _entityReadRepository.GetQuery();

            Assert.Empty(dbEntities);
        }

        [Fact]
        public async Task TestRepositoryFind()
        {
            var entities = _entityReadRepository.Find(x => x.Age > 80);

            Assert.Empty(entities.ToList());

            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            entity1.Age = 79;
            entity2.Age = 81;

            await _context.Entities.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            entities = _entityReadRepository.Find(x => x.Age > 80);

            Assert.Single(entities.ToList());

            entity1.Age = 85;
            _context.Entities.Update(entity1);
            await _context.SaveChangesAsync();

            entities = _entityReadRepository.Find(x => x.Age > 80);

            Assert.Equal(2, entities.ToList().Count);

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();

            entities = _entityReadRepository.Find(x => x.Age > 80);

            Assert.Empty(entities.ToList());
        }

        [Fact]
        public async Task TestRepositoryWhere()
        {
            var entities = await _entityReadRepository.WhereAsync(x => x.Age > 80);

            Assert.Equal(0, entities.Count);

            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            entity1.Age = 79;
            entity2.Age = 81;

            await _context.Entities.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            entities = await _entityReadRepository.WhereAsync(x => x.Name == entity2.Name);

            Assert.Equal(1, entities.Count);

            entities = await _entityReadRepository.WhereAsync(x => x.Age > 80);

            Assert.Equal(1, entities.Count);

            entity1.Age = 85;
            _context.Entities.Update(entity1);
            await _context.SaveChangesAsync();

            entities = await _entityReadRepository.WhereAsync(x => x.Age > 80);

            Assert.Equal(2, entities.Count);

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();

            entities = await _entityReadRepository.WhereAsync(x => x.Age > 80);

            Assert.Equal(0, entities.Count);
        }

        [Fact]
        public async Task TestRepositoryGetById()
        {
            var nullEntity = await _entityReadRepository.GetByIdAsync(Guid.NewGuid());

            Assert.Null(nullEntity);

            // ReSharper disable once HeuristicUnreachableCode
            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();

            await _context.Entities.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            var dbEntity1 = await _entityReadRepository.GetByIdAsync(entity1.Id);
            Assert.Equal(entity1.Name, dbEntity1.Name);

            var dbEntity2 = await _entityReadRepository.GetByIdAsync(entity2.Id);
            Assert.Equal(entity2.Name, dbEntity2.Name);

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();
            Assert.Equal(0, await _entityReadRepository.CountAsync());

            nullEntity = await _entityReadRepository.GetByIdAsync(entity2.Id);

            Assert.Null(nullEntity);
        }

        [Fact]
        public async Task TestRepositorySingleOrDefault()
        {
            var nullEntity = await _entityReadRepository.SingleOrDefaultAsync(x => x.Age > 80);

            Assert.Null(nullEntity);

            // ReSharper disable once HeuristicUnreachableCode
            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            entity1.Age = 79;
            entity2.Age = 81;

            await _context.Entities.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            var dbEntity = await _entityReadRepository.SingleOrDefaultAsync(x => x.Age > 80);

            Assert.Equal(entity2.Id, dbEntity.Id);
            Assert.Equal(entity2.Name, dbEntity.Name);

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _entityReadRepository.SingleOrDefaultAsync(x => x.Age > 75);
            });

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(0, await _entityReadRepository.CountAsync());
        }

        [Fact]
        public async Task TestRepositoryFirstOrDefault()
        {
            var nullEntity = await _entityReadRepository.FirstOrDefaultAsync(x => x.Age > 80);

            Assert.Null(nullEntity);

            // ReSharper disable once HeuristicUnreachableCode
            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            entity1.Age = 79;
            entity2.Age = 81;

            await _context.Entities.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            var entity = await _entityReadRepository.FirstOrDefaultAsync(x => x.Age > 80);

            Assert.Equal(entity2.Id, entity.Id);
            Assert.Equal(entity2.Name, entity.Name);

            entity = await _entityReadRepository.FirstOrDefaultAsync(x => x.Age > 75);

            Assert.Equal(entity1.Id, entity.Id);
            Assert.Equal(entity1.Name, entity.Name);

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(0, await _entityReadRepository.CountAsync());
        }

        [Fact]
        public async Task TestRepositoryExists()
        {
            Assert.False(await _entityReadRepository.ExistsAsync(x => x.Age >= 75));

            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            var entity3 = GenerateTestEntity();
            entity1.Age = 70;
            entity2.Age = 76;
            entity3.Age = 80;

            await _context.Entities.AddRangeAsync(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            Assert.True(await _entityReadRepository.ExistsAsync(x => x.Age >= 75));
            Assert.True(await _entityReadRepository.ExistsAsync(x => x.Age >= 80));
            Assert.False(await _entityReadRepository.ExistsAsync(x => x.Age >= 85));

            entity3.Age = 85;
            _context.Entities.Update(entity3);
            await _context.SaveChangesAsync();

            Assert.True(await _entityReadRepository.ExistsAsync(x => x.Age >= 75));
            Assert.True(await _entityReadRepository.ExistsAsync(x => x.Age >= 80));
            Assert.True(await _entityReadRepository.ExistsAsync(x => x.Age >= 85));

            _context.Entities.RemoveRange(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            Assert.False(await _entityReadRepository.ExistsAsync(x => x.Age >= 75));
        }

        [Fact]
        public async Task TestRepositoryCount()
        {
            Assert.Equal(0, await _entityReadRepository.CountAsync());

            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            var entity3 = GenerateTestEntity();

            await _context.Entities.AddRangeAsync(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            Assert.Equal(3, await _entityReadRepository.CountAsync());

            var entity4 = GenerateTestEntity();

            await _context.Entities.AddAsync(entity4);
            await _context.SaveChangesAsync();

            Assert.Equal(4, await _entityReadRepository.CountAsync());

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, await _entityReadRepository.CountAsync());

            _context.Entities.Remove(entity3);
            await _context.SaveChangesAsync();

            Assert.Equal(1, await _entityReadRepository.CountAsync());

            _context.Entities.Remove(entity4);
            await _context.SaveChangesAsync();

            Assert.Equal(0, await _entityReadRepository.CountAsync());
        }

        [Fact]
        public async Task TestRepositoryCountWithPredicate()
        {
            Assert.Equal(0, await _entityReadRepository.CountAsync(x => x.Age > 75));

            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            var entity3 = GenerateTestEntity();
            entity1.Age = 70;
            entity2.Age = 76;
            entity3.Age = 80;

            await _context.Entities.AddRangeAsync(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            Assert.Equal(3, await _entityReadRepository.CountAsync(x => x.Age > 65));
            Assert.Equal(2, await _entityReadRepository.CountAsync(x => x.Age > 75));
            Assert.Equal(1, await _entityReadRepository.CountAsync(x => x.Age < 75));
            Assert.Equal(0, await _entityReadRepository.CountAsync(x => x.Age < 70));
            Assert.Equal(1, await _entityReadRepository.CountAsync(x => x.Age <= 70));
            Assert.Equal(0, await _entityReadRepository.CountAsync(x => x.Age > 80));

            entity2.Age = 72;
            _context.Entities.Update(entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(1, await _entityReadRepository.CountAsync(x => x.Age > 75));
            Assert.Equal(2, await _entityReadRepository.CountAsync(x => x.Age < 75));

            _context.Entities.RemoveRange(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            Assert.Equal(0, await _entityReadRepository.CountAsync(x => x.Age > 65));
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
