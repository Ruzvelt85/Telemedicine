using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetSaturationMeasurementListQueryHandlerTests
    {
        private readonly HealthMeasurementDomainServiceTestDbContext _context;
        private readonly GetSaturationMeasurementListQueryHandler _queryHandler;

        public GetSaturationMeasurementListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthMeasurementDomainServiceTestDbContext>()
                .UseInMemoryDatabase($"GetSaturationMeasurementListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthMeasurementDomainServiceTestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetSaturationMeasurementListQueryHandler(mapper, new SaturationMeasurementReadRepository(_context));
        }

        [Fact]
        public async Task EmptyRepository_ShouldReturnEmptySaturationMeasurementList()
        {
            var result = await _queryHandler.HandleAsync(new GetMeasurementListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task DefaultQuery_ShouldReturnEmptySaturationMeasurementList()
        {
            var saturationMeasurement = new Fixture().Build<SaturationMeasurement>().Create();
            await _context.AddAsync(saturationMeasurement);
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

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(DateTime.Now.AddDays(-1), patientId),
                MockSaturationMeasurement(DateTime.Now, patientId, id),
                MockSaturationMeasurement(DateTime.Now.AddDays(-2), patientId)
            };

            await _context.AddRangeAsync(saturationMeasurement);
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
            Assert.Equal(result.Items.First().Id, id);
        }

        [Fact]
        public async Task QueryWithPaging_ShouldReturnSpecificItemsCount()
        {
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: patientId)
            };
            await _context.AddRangeAsync(saturationMeasurement);
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
        }

        [Fact]
        public async Task QueryWithPaging_SkipWithTake()
        {
            var now = DateTime.UtcNow;
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(now, patientId),
                MockSaturationMeasurement(now.AddDays(-1), patientId),
                MockSaturationMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(saturationMeasurement);
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
            Assert.Equal(result.Items.First().ClientDate, now.AddDays(-2));
        }

        [Fact]
        public async Task QueryWithFilter_WithoutPatientId()
        {
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: patientId)
            };
            await _context.AddRangeAsync(saturationMeasurement);
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

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: patientId),
                MockSaturationMeasurement(patientId: Guid.NewGuid())
            };
            var exprectedSaturationMeasurement = saturationMeasurement.Where(el => el.PatientId == patientId);
            await _context.AddRangeAsync(saturationMeasurement);
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
            Assert.All(result.Items, el => Assert.Contains(exprectedSaturationMeasurement, measurement => el.Id == measurement.Id));
        }

        [Fact]
        public async Task QueryWithFilter_EmptyDateFilter()
        {
            var now = DateTime.UtcNow;
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(now, patientId),
                MockSaturationMeasurement(now.AddDays(-1), patientId),
                MockSaturationMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(saturationMeasurement);
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
            var now = DateTime.UtcNow;
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(now, patientId),
                MockSaturationMeasurement(now.AddDays(-1), patientId),
                MockSaturationMeasurement(now.AddDays(-2), patientId)
            };
            var exprectedSaturationMeasurement = saturationMeasurement.Where(el => el.ClientDate >= now.AddDays(-1));
            await _context.AddRangeAsync(saturationMeasurement);
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
            Assert.All(result.Items, el => Assert.Contains(exprectedSaturationMeasurement, measurement => el.Id == measurement.Id));
        }

        [Fact]
        public async Task QueryWithFilter_OnlyToDateFilter()
        {
            var now = DateTime.UtcNow;
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(now, patientId),
                MockSaturationMeasurement(now.AddDays(-1), patientId),
                MockSaturationMeasurement(now.AddDays(-2), patientId)
            };
            var exprectedSaturationMeasurement = saturationMeasurement.Where(el => el.ClientDate <= now);
            await _context.AddRangeAsync(saturationMeasurement);
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
            Assert.All(result.Items, el => Assert.Contains(exprectedSaturationMeasurement, measurement => el.Id == measurement.Id));
        }

        [Fact]
        public async Task QueryWithFilter_FullDateFilter()
        {
            var now = DateTime.UtcNow;
            var patientId = Guid.NewGuid();

            var saturationMeasurement = new List<SaturationMeasurement>
            {
                MockSaturationMeasurement(now, patientId),
                MockSaturationMeasurement(now.AddDays(3), patientId),
                MockSaturationMeasurement(now.AddDays(-1), patientId),
                MockSaturationMeasurement(now.AddDays(-2), patientId),
                MockSaturationMeasurement(now.AddDays(-5), patientId)
            };
            await _context.AddRangeAsync(saturationMeasurement);
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

        private SaturationMeasurement MockSaturationMeasurement(DateTime clientDate = default,
            Guid patientId = default, Guid id = default)
        {
            return new Fixture().Build<SaturationMeasurement>()
                .With(_ => _.ClientDate, clientDate)
                .With(_ => _.Id, id)
                .With(_ => _.PatientId, patientId)
                .Create();
        }
    }
}
