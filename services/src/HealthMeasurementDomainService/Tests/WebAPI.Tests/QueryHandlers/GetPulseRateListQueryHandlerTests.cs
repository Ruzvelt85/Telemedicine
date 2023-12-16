﻿using System;
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
    public class GetPulseRateListQueryHandlerTests
    {
        private readonly HealthMeasurementDomainServiceTestDbContext _context;
        private readonly GetPulseRateListQueryHandler _queryHandler;

        public GetPulseRateListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthMeasurementDomainServiceTestDbContext>()
                .UseInMemoryDatabase($"GetPulseRateListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthMeasurementDomainServiceTestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetPulseRateListQueryHandler(mapper, new PulseRateMeasurementReadRepository(_context));
        }

        [Fact]
        public async Task EmptyRepository_ShouldReturnEmptyList()
        {
            var result = await _queryHandler.HandleAsync(new GetMeasurementListQuery());

            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task DefaultQuery_ShouldReturnEmptyList()
        {
            var measurement = new Fixture().Build<PulseRateMeasurement>().Create();
            await _context.AddAsync(measurement);
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

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(DateTime.Now, patientId, id),
                MockPulseRateMeasurement(DateTime.Now.AddDays(-1), patientId),
                MockPulseRateMeasurement(DateTime.Now.AddDays(-2), patientId)
            };

            await _context.AddRangeAsync(measurements);
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

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(patientId: patientId),
                MockPulseRateMeasurement(patientId: patientId),
                MockPulseRateMeasurement(patientId: patientId),
                MockPulseRateMeasurement(patientId: patientId)
            };
            await _context.AddRangeAsync(measurements);
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

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(now, patientId),
                MockPulseRateMeasurement(now.AddDays(-1), patientId),
                MockPulseRateMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(measurements);
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
            Assert.Equal(3, result.Paging.Total);
        }

        [Fact]
        public async Task QueryWithFilter_WithoutPatientId()
        {
            var patientId = Guid.NewGuid();

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(patientId: patientId),
                MockPulseRateMeasurement(patientId: patientId),
                MockPulseRateMeasurement(patientId: patientId)
            };
            await _context.AddRangeAsync(measurements);
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
            var expectedIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(patientId: patientId, id: expectedIds[0]),
                MockPulseRateMeasurement(patientId: patientId, id: expectedIds[1]),
                MockPulseRateMeasurement(patientId: Guid.NewGuid())
            };
            await _context.AddRangeAsync(measurements);
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
            Assert.True(expectedIds.All(id => result.Items.Any(i => i.Id == id)));
        }

        [Fact]
        public async Task QueryWithFilter_EmptyDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();
            var expectedIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(now, patientId, expectedIds[0]),
                MockPulseRateMeasurement(now.AddDays(-1), patientId, expectedIds[1]),
                MockPulseRateMeasurement(now.AddDays(-2), patientId, expectedIds[2])
            };
            await _context.AddRangeAsync(measurements);
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
            Assert.True(expectedIds.All(id => result.Items.Any(i => i.Id == id)));
        }

        [Fact]
        public async Task QueryWithFilter_OnlyFromDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();
            var expectedIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(now, patientId, expectedIds[0]),
                MockPulseRateMeasurement(now.AddDays(-1), patientId, expectedIds[1]),
                MockPulseRateMeasurement(now.AddDays(-2), patientId)
            };
            await _context.AddRangeAsync(measurements);
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
            Assert.True(expectedIds.All(id => result.Items.Any(i => i.Id == id)));
        }

        [Fact]
        public async Task QueryWithFilter_OnlyToDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();
            var expectedIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(now, patientId),
                MockPulseRateMeasurement(now.AddDays(-1), patientId, expectedIds[0]),
                MockPulseRateMeasurement(now.AddDays(-2), patientId, expectedIds[1])
            };
            await _context.AddRangeAsync(measurements);
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
            Assert.True(expectedIds.All(id => result.Items.Any(i => i.Id == id)));
        }

        [Fact]
        public async Task QueryWithFilter_FullDateFilter()
        {
            var now = DateTime.Now;
            var patientId = Guid.NewGuid();
            var expectedIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var measurements = new List<PulseRateMeasurement>
            {
                MockPulseRateMeasurement(now.AddDays(3), patientId),
                MockPulseRateMeasurement(now, patientId, expectedIds[0]),
                MockPulseRateMeasurement(now.AddDays(-1), patientId, expectedIds[1]),
                MockPulseRateMeasurement(now.AddDays(-2), patientId, expectedIds[2]),
                MockPulseRateMeasurement(now.AddDays(-5), patientId)
            };
            await _context.AddRangeAsync(measurements);
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
            Assert.True(expectedIds.All(id => result.Items.Any(i => i.Id == id)));
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        private PulseRateMeasurement MockPulseRateMeasurement(DateTime clientDate = default,
            Guid patientId = default, Guid id = default)
        {
            return new Fixture().Build<PulseRateMeasurement>()
                .With(_ => _.ClientDate, clientDate)
                .With(_ => _.Id, id)
                .With(_ => _.PatientId, patientId)
                .Create();
        }
    }
}
