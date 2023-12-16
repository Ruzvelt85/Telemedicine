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
    public class WriteRepositoryTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly TestEntityReadRepository _readRepository;
        private readonly TestEntityWriteRepository _writeRepository;
        private readonly TestLogicallyDeletableAuditableWriteRepository _testLogicallyDeletableWriteRepository;

        public WriteRepositoryTests()
        {
            // For every unit test we have to specify different name of InMemory database
            // in order to prevent unstable results of tests due to only one service-provider
            var dbContextOptions =
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase($"RepositoriesTests-{Guid.NewGuid()}");

            _context = new TestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            _context.Database.EnsureCreated();

            _readRepository = new TestEntityReadRepository(_context);
            _writeRepository = new TestEntityWriteRepository(_context);

            _testLogicallyDeletableWriteRepository = new TestLogicallyDeletableAuditableWriteRepository(_context);
        }

        [Fact]
        public async Task TestRepositoryAdd()
        {
            var entity1 = GenerateTestEntity();

            await _writeRepository.AddAsync(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.Entities.Count());

            var entity2 = GenerateTestEntity();

            await _writeRepository.AddAsync(entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.Entities.Count());

            _context.Entities.Remove(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.Entities.Count());

            _context.Entities.Remove(entity2);
            await _context.SaveChangesAsync();

            Assert.Empty((await _readRepository.GetAllAsync()));
        }

        [Fact]
        public async Task TestRepositoryAddRange()
        {
            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            var entity3 = GenerateTestEntity();

            Assert.Equal(0, _context.Entities.Count());

            await _writeRepository.AddRangeAsync(new[] { entity1, entity2, entity3 });
            await _context.SaveChangesAsync();

            Assert.Equal(3, _context.Entities.Count());

            var entity4 = GenerateTestEntity();

            await _writeRepository.AddRangeAsync(new[] { entity4 });
            await _context.SaveChangesAsync();

            Assert.Equal(4, _context.Entities.Count());

            _context.Entities.RemoveRange(entity1, entity2, entity3, entity4);
            await _context.SaveChangesAsync();

            Assert.Empty((await _readRepository.GetAllAsync()));
        }

        [Fact]
        public async Task TestRepositoryUpdate()
        {
            Assert.Equal(0, _context.Entities.Count());

            var entity1 = GenerateTestEntity();
            await _context.Entities.AddAsync(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.Entities.Count());

            var nameBeforeUpdate = entity1.Name;

            entity1.Name = "Jack Smith Jr.";

            await _writeRepository.UpdateAsync(entity1);
            await _context.SaveChangesAsync();

            var updatedEntity = (await _readRepository.GetAllAsync()).First();

            Assert.NotEqual(nameBeforeUpdate, updatedEntity.Name);
            Assert.Equal(entity1.Name, updatedEntity.Name);

            _context.Entities.Remove(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(0, await _readRepository.CountAsync());
        }

        [Fact]
        public async Task TestRepositoryUpdateRange()
        {
            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();

            await _context.Entities.AddRangeAsync(entity1, entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.Entities.Count());

            var nameBeforeUpdate1 = entity1.Name;
            var nameBeforeUpdate2 = entity2.Name;

            entity1.Name = "Jack Smith Jr.";
            entity2.Name = "John Wolf Sr.";

            await _writeRepository.UpdateRangeAsync(new[] { entity1, entity2 });
            await _context.SaveChangesAsync();

            var updatedEntities = await _readRepository.GetAllAsync();
            var updatedEntity1 = updatedEntities.First(x => x.Id == entity1.Id);
            var updatedEntity2 = updatedEntities.First(x => x.Id == entity2.Id);

            Assert.NotEqual(nameBeforeUpdate1, updatedEntity1.Name);
            Assert.NotEqual(nameBeforeUpdate2, updatedEntity2.Name);
            Assert.Equal(entity1.Name, updatedEntity1.Name);
            Assert.Equal(entity2.Name, updatedEntity2.Name);

            _context.Entities.RemoveRange(entity1, entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(0, await _readRepository.CountAsync());
        }

        [Fact]
        public async Task TestRepositoryDelete()
        {
            var entity1 = GenerateTestEntity();
            await _context.Entities.AddAsync(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.Entities.Count());

            var entity2 = GenerateTestEntity();
            await _context.Entities.AddAsync(entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.Entities.Count());

            await _writeRepository.DeleteAsync(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.Entities.Count());

            await _writeRepository.DeleteAsync(entity2);
            await _context.SaveChangesAsync();

            Assert.Empty((await _readRepository.GetAllAsync()));

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _writeRepository.DeleteAsync(entity2);
                await _context.SaveChangesAsync();
            });
        }

        [Fact]
        public async Task TestRepositoryLogicalDelete()
        {
            var entity1 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.LogicallyDeletedAuditableEntities.Count());

            var entity2 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.LogicallyDeletedAuditableEntities.Count());

            await _testLogicallyDeletableWriteRepository.DeleteAsync(entity1);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.LogicallyDeletedAuditableEntities.Count());
            Assert.True(entity1.IsDeleted);
            Assert.False(entity2.IsDeleted);

            await _testLogicallyDeletableWriteRepository.DeleteAsync(entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.LogicallyDeletedAuditableEntities.Count());
            Assert.True(entity1.IsDeleted);
            Assert.True(entity2.IsDeleted);

            //so far if we try to delete again no exeption should be thrown
            await _testLogicallyDeletableWriteRepository.DeleteAsync(entity2);
            await _context.SaveChangesAsync();

            Assert.Equal(2, _context.LogicallyDeletedAuditableEntities.Count());
            Assert.True(entity1.IsDeleted);
            Assert.True(entity2.IsDeleted);
        }

        [Fact]
        public async Task TestRepositoryDeleteRange()
        {
            var entity1 = GenerateTestEntity();
            var entity2 = GenerateTestEntity();
            var entity3 = GenerateTestEntity();

            await _context.Entities.AddRangeAsync(entity1, entity2, entity3);
            await _context.SaveChangesAsync();

            Assert.Equal(3, _context.Entities.Count());

            await _writeRepository.DeleteRangeAsync(new[] { entity3, entity2 });
            await _context.SaveChangesAsync();

            Assert.Equal(1, _context.Entities.Count());

            await _writeRepository.DeleteRangeAsync(new[] { entity1 });
            await _context.SaveChangesAsync();

            Assert.Empty((await _readRepository.GetAllAsync()));

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
            {
                await _writeRepository.DeleteRangeAsync(new[] { entity1 });
                await _context.SaveChangesAsync();
            });
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

        private TestLogicallyDeletedAuditableEntity GenerateAuditableLogicallyDeletableEntity()
        {
            var fakeEntity = new Faker<TestLogicallyDeletedAuditableEntity>()
                .RuleFor(p => p.Id, _ => Guid.NewGuid())
                .RuleFor(p => p.Test, f => f.Name.FullName())
                .RuleFor(p => p.Timestamp, f => f.Random.UInt());

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
