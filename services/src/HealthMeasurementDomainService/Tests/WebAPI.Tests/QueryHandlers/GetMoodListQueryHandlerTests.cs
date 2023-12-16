using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetMoodListQueryHandlerTests
    {
        private readonly HealthMeasurementDomainServiceTestDbContext _context;
        private readonly GetMoodListQueryHandler _queryHandler;

        public GetMoodListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthMeasurementDomainServiceTestDbContext>()
                .UseInMemoryDatabase($"GetMoodListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthMeasurementDomainServiceTestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetMoodListQueryHandler(mapper, new MoodMeasurementReadRepository(_context));
        }

        [Fact]
        public async Task EmptyRepository_ShouldReturnEmptyMoodList()
        {
            var result = await _queryHandler.HandleAsync(new GetMeasurementListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task DefaultQuery_ShouldReturnEmptyMoodList()
        {
            var appointment = new Fixture().Build<MoodMeasurement>().Create();
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var result = await _queryHandler.HandleAsync(new GetMeasurementListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task QueryWithPaging_ShouldSortByClientDate()
        {
            var id = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(DateTime.Now, patientId, id),
                MockMoodMeasurement(DateTime.Now.AddDays(-1), patientId),
                MockMoodMeasurement(DateTime.Now.AddDays(-2), patientId)
            };

            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Paging = new PagingRequestDto(1),
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(1, result.Items.Count);
            Assert.Equal(result.Items.FirstOrDefault()!.Id, id);
        }

        [Fact]
        public async Task QueryWithPaging_ShouldReturnSpecificItemsCount()
        {
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Paging = new PagingRequestDto(2),
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(4, result.Paging.Total);
        }

        [Fact]
        public async Task QueryWithPaging_SkipWithTake()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(now, patientId),
                MockMoodMeasurement(now.AddDays(-1), patientId),
                MockMoodMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Paging = new PagingRequestDto(1, 2),
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(1, result.Items.Count);
            Assert.Equal(result.Items.FirstOrDefault()!.ClientDate, now.AddDays(-2));
            Assert.Equal(3, result.Paging.Total);
        }

        [Fact]
        public async Task QueryWithFilter_WithoutPatientId()
        {
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery();

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task QueryWithFilter_WithPatientId()
        {
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: patientId),
                MockMoodMeasurement(patientId: Guid.NewGuid())
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task QueryWithFilter_EmptyDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(now, patientId),
                MockMoodMeasurement(now.AddDays(-1), patientId),
                MockMoodMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId,
                    DateRange = new Range<DateTime?>()
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
        }

        [Fact]
        public async Task QueryWithFilter_OnlyFromDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(now, patientId),
                MockMoodMeasurement(now.AddDays(-1), patientId),
                MockMoodMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId,
                    DateRange = new Range<DateTime?>(now.AddDays(-1), null)
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task QueryWithFilter_OnlyToDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(now, patientId),
                MockMoodMeasurement(now.AddDays(-1), patientId),
                MockMoodMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId,
                    DateRange = new Range<DateTime?>(null, now)
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task QueryWithFilter_FullDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();

            var moods = new List<MoodMeasurement>
            {
                MockMoodMeasurement(now, patientId),
                MockMoodMeasurement(now.AddDays(3), patientId),
                MockMoodMeasurement(now.AddDays(-1), patientId),
                MockMoodMeasurement(now.AddDays(-2), patientId),
                MockMoodMeasurement(now.AddDays(-5), patientId)
            };
            await _context.AddRangeAsync(moods);
            await _context.SaveChangesAsync();

            var query = new GetMeasurementListQuery
            {
                Filter = new MeasurementListFilterRequestDto
                {
                    PatientId = patientId,
                    DateRange = new Range<DateTime?>(now.AddDays(-3), now.AddDays(1))
                }
            };

            var result = await _queryHandler.HandleAsync(query);

            Assert.NotNull(result);
            Assert.Equal(3, result.Items.Count);
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        private MoodMeasurement MockMoodMeasurement(DateTime clientDate = default,
            Guid patientId = default, Guid id = default)
        {
            return new Fixture().Build<MoodMeasurement>()
                .With(_ => _.ClientDate, clientDate)
                .With(_ => _.Id, id)
                .With(_ => _.PatientId, patientId)
                .Create();
        }
    }
}
