using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests
{
    public class DbContextTests : IDisposable
    {
        private readonly TimeSpan _precision = TimeSpan.FromSeconds(1);
        private readonly TestDbContext _context;

        public DbContextTests()
        {
            // For every unit test we have to specify different name of InMemory database
            // in order to prevent unstable results of tests due to only one service-provider
            var dbContextOptions =
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase($"RepositoriesTests-{Guid.NewGuid()}");

            _context = new TestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task TestDbContextAddAuditable()
        {
            var entity1 = GenerateAuditableLogicallyDeletableEntity();
            var entity2 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity1);
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity2);

            entity1.UpdatedOn.Should().Be(DateTime.MinValue);
            entity2.UpdatedOn.Should().Be(DateTime.MinValue);
            entity1.CreatedOn.Should().Be(DateTime.MinValue);
            entity2.CreatedOn.Should().Be(DateTime.MinValue);

            var creationTime = GetDateTime();
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);

            var updatedEntity1 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity1.Id);
            var updatedEntity2 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity2.Id);

            updatedEntity1.UpdatedOn.Should().BeCloseTo(creationTime, _precision);
            updatedEntity2.UpdatedOn.Should().BeCloseTo(creationTime, _precision);
            updatedEntity1.CreatedOn.Should().BeCloseTo(creationTime, _precision);
            updatedEntity2.CreatedOn.Should().BeCloseTo(creationTime, _precision);
        }

        [Fact]
        public async Task TestDbContextUpdateAuditable()
        {
            var entity1 = GenerateAuditableLogicallyDeletableEntity();
            var entity2 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity1);
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity2);

            entity1.UpdatedOn.Should().Be(DateTime.MinValue);
            entity2.UpdatedOn.Should().Be(DateTime.MinValue);
            entity1.CreatedOn.Should().Be(DateTime.MinValue);
            entity2.CreatedOn.Should().Be(DateTime.MinValue);

            var creationTime = GetDateTime();
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);

            var createdEntity1 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity1.Id);
            var createdEntity2 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity2.Id);

            createdEntity1.UpdatedOn.Should().BeCloseTo(creationTime, _precision);
            createdEntity2.UpdatedOn.Should().BeCloseTo(creationTime, _precision);
            createdEntity1.CreatedOn.Should().BeCloseTo(creationTime, _precision);
            createdEntity2.CreatedOn.Should().BeCloseTo(creationTime, _precision);

            entity1.Test = "updated";
            var entity1CreationTime = entity1.CreatedOn;
            var entity2CreationTime = entity2.CreatedOn;
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);

            var updatedEntity1 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity1.Id);
            var updatedEntity2 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity2.Id);

            updatedEntity1.CreatedOn.Should().Be(entity1CreationTime, "Creation time does not change");
            updatedEntity1.UpdatedOn.Should().BeAfter(entity1CreationTime, "Entity has been changed");
            updatedEntity2.CreatedOn.Should().Be(entity2CreationTime, "Entity has not been changed");
            updatedEntity2.UpdatedOn.Should().Be(entity2CreationTime, "Entity has not been changed");
        }

        [Fact]
        public async Task TestDbContextLogicalDelete()
        {
            var entity1 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity1);
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(1);

            var entity2 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity2);
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);

            _context.LogicallyDeletedAuditableEntities.Remove(entity1);
            await _context.SaveChangesAsync();

            var updatedEntity1 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity1.Id);
            var updatedEntity2 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity2.Id);

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);
            updatedEntity1.IsDeleted.Should().BeTrue("Entity has been deleted");
            updatedEntity2.IsDeleted.Should().BeFalse("Entity has not been deleted");

            _context.LogicallyDeletedAuditableEntities.Remove(entity2);
            await _context.SaveChangesAsync();

            updatedEntity1 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity1.Id);
            updatedEntity2 = await _context.LogicallyDeletedAuditableEntities.FirstAsync(e => e.Id == entity2.Id);
            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);
            updatedEntity1.IsDeleted.Should().BeTrue("Entity has been deleted");
            updatedEntity2.IsDeleted.Should().BeTrue("Entity has been deleted");
        }

        [Fact]
        public async Task TestDbContextExplicitLogicalDelete()
        {
            var entity1 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity1);
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(1);

            var entity2 = GenerateAuditableLogicallyDeletableEntity();
            await _context.LogicallyDeletedAuditableEntities.AddAsync(entity2);
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);

            entity1.IsDeleted = true;
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);
            _context.LogicallyDeletedAuditableEntities.First(e => e.Id == entity1.Id).IsDeleted.Should().BeTrue();
            _context.LogicallyDeletedAuditableEntities.First(e => e.Id == entity2.Id).IsDeleted.Should().BeFalse();

            entity2.IsDeleted = true;
            await _context.SaveChangesAsync();

            _context.LogicallyDeletedAuditableEntities.Should().HaveCount(2);
            _context.LogicallyDeletedAuditableEntities.First(e => e.Id == entity1.Id).IsDeleted.Should().BeTrue();
            _context.LogicallyDeletedAuditableEntities.First(e => e.Id == entity2.Id).IsDeleted.Should().BeTrue();
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

        private static DateTime GetDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}
