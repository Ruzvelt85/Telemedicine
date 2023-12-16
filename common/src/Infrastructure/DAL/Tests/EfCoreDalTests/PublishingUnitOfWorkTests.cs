using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Event;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.Simple;
using Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests.TestSetup.HasDomainEvents;

namespace Telemedicine.Common.Infrastructure.DAL.Tests.EfCoreDalTests
{
    public class PublishingUnitOfWorkTests
    {
        [Fact]
        public async Task SaveAsync_WhenEntityDoNotHaveDomainEvent_ShouldNotDispatchEvents()
        {
            // Arrange
            using var context = GetTestDbContext();
            await context.Database.EnsureCreatedAsync();
            var domainEventPublisherSpy = new DomainEventPublisherSpy();

            var repository = new TestSimpleEntityWriteRepository(context);
            var entityDoNotHaveDomainEvent = new TestSimpleEntity(Guid.NewGuid());
            await repository.AddAsync(entityDoNotHaveDomainEvent);

            var unitOfWork = new UnitOfWork<TestDbContext>(context, domainEventPublisherSpy);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task SaveAsync_WhenNoEntities_ShouldNotDispatchEvents()
        {
            // Arrange
            using var context = GetTestDbContext();
            await context.Database.EnsureCreatedAsync();
            var domainEventPublisherSpy = new DomainEventPublisherSpy();

            var unitOfWork = new UnitOfWork<TestDbContext>(context, domainEventPublisherSpy);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task SaveAsync_WhenNoDomainEvents_ShouldNotDispatchEvents()
        {
            // Arrange
            using var context = GetTestDbContext();
            await context.Database.EnsureCreatedAsync();
            var domainEventPublisherSpy = new DomainEventPublisherSpy();

            var repository = new TestHasDomainEventsEntityWriteRepository(context);
            await repository.AddAsync(new TestHasDomainEventsEntity());

            var unitOfWork = new UnitOfWork<TestDbContext>(context, domainEventPublisherSpy);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task SaveAsync_WhenDomainEventAlreadyPublished_ShouldNotDispatchEvents()
        {
            // Arrange
            using var context = GetTestDbContext();
            await context.Database.EnsureCreatedAsync();
            var domainEventPublisherSpy = new DomainEventPublisherSpy();

            var entityWithOneDomainEvent = new TestHasDomainEventsEntity();
            entityWithOneDomainEvent.AddDomainEvent(new TestDomainEvent() { IsPublished = true });

            var repository = new TestHasDomainEventsEntityWriteRepository(context);
            await repository.AddAsync(entityWithOneDomainEvent);

            var unitOfWork = new UnitOfWork<TestDbContext>(context, domainEventPublisherSpy);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            domainEventPublisherSpy.ShouldPublishNumberOfEvents(0);
        }

        [Fact]
        public async Task SaveAsync_WhenDomainEvent_ShouldDispatchEvents()
        {
            // Arrange
            using var context = GetTestDbContext();
            await context.Database.EnsureCreatedAsync();
            var domainEventPublisherSpy = new DomainEventPublisherSpy();

            var entityWithOneDomainEvent = new TestHasDomainEventsEntity();
            entityWithOneDomainEvent.AddDomainEvent(new TestDomainEvent());

            var repository = new TestHasDomainEventsEntityWriteRepository(context);
            await repository.AddAsync(entityWithOneDomainEvent);

            var unitOfWork = new UnitOfWork<TestDbContext>(context, domainEventPublisherSpy);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            domainEventPublisherSpy.ShouldPublishNumberOfEvents(1)
                .ShouldPublishAllEvents();
        }

        [Fact]
        public async Task SaveAsync_WhenUpdateEntity_ShouldNotDispatchTwice()
        {
            // Arrange
            using var context = GetTestDbContext();
            await context.Database.EnsureCreatedAsync();
            var domainEventPublisherSpy = new DomainEventPublisherSpy();
            var unitOfWork = new UnitOfWork<TestDbContext>(context, domainEventPublisherSpy);

            var entityWithThreeDomainEvents = new TestHasDomainEventsEntity();
            entityWithThreeDomainEvents.AddDomainEvent(new TestDomainEvent());

            var repository = new TestHasDomainEventsEntityWriteRepository(context);
            await repository.AddAsync(entityWithThreeDomainEvents);

            await unitOfWork.SaveAsync();

            entityWithThreeDomainEvents.AddDomainEvent(new TestDomainEvent());

            await repository.UpdateAsync(entityWithThreeDomainEvents);

            // Act
            await unitOfWork.SaveAsync();

            // Assert
            domainEventPublisherSpy.ShouldPublishNumberOfEvents(2)
                .ShouldPublishAllEvents();
        }

        private static TestDbContext GetTestDbContext()
        {
            var dbContextOptions =
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase($"{nameof(PublishingUnitOfWorkTests)}-{Guid.NewGuid()}");

            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            var context = new TestDbContext(dbContextOptions.Options, mockOptions.Object, new NullLoggerFactory());

            return context;
        }
    }
}
