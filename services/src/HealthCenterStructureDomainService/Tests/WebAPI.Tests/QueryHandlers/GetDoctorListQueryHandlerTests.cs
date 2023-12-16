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
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Repositories.User;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.Tests.QueryHandlers
{
    public class GetDoctorListQueryHandlerTests
    {
        private readonly GetDoctorListQueryHandler _queryHandler;
        private readonly PagingRequestDto _defaultPagingRequest = new() { Take = 10, Skip = 0 };
        private readonly HealthCenterStructureDomainServiceDbContext _context;

        public GetDoctorListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<HealthCenterStructureDomainServiceDbContext>()
                .UseInMemoryDatabase($"GetDoctorListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new HealthCenterStructureDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());

            var doctorRepository = new DoctorReadRepository(_context);

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetDoctorListQueryHandler(mapper, doctorRepository);
        }

        [Fact]
        public async Task EmptyDoctorsRepository_WithoutFilter_Test()
        {
            var filterRequest = new DoctorListFilterRequestDto();
            var query = new GetDoctorListQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);

            var response = await _queryHandler.HandleAsync(query);

            Assert.Empty(response.Items);
        }

        [Fact]
        public async Task GetDoctorList_WithoutFilters_Successfully_Test()
        {
            // Arrange
            var (healthCenterId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var healthCenters = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId).Create()
            };
            var doctors = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(doctorId).Create().SetHealthCenters(healthCenters)
            };

            await _context.AddRangeAsync(healthCenters);
            await _context.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto();
            var query = new GetDoctorListQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var response = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.Single(response.Items);
            var doctorInResponse = response.Items.ToArray()[0];
            Assert.Equal(doctorId, doctorInResponse.Id);
            Assert.Single(doctorInResponse.HealthCenters);
            Assert.Equal(healthCenterId, doctorInResponse.HealthCenters.First().Id);
        }

        [Fact]
        public async Task GetDoctorList_WithFilters_ByName_Successfully_Test()
        {
            // Arrange
            var (healthCenterId, doctorId) = (Guid.NewGuid(), Guid.NewGuid());
            var healthCenters = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId).Create()
            };
            var doctors = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(doctorId, firstName: "Ameli", lastName: "Seidu").Create().SetHealthCenters(healthCenters),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await _context.AddRangeAsync(healthCenters);
            await _context.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto { Name = "Ameli" };
            var query = new GetDoctorListQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var response = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.Single(response.Items);
            var doctorInResponse = response.Items.ToArray()[0];
            Assert.Equal(doctorId, doctorInResponse.Id);
            Assert.Equal("Ameli", doctorInResponse.FirstName);
            Assert.Equal("Seidu", doctorInResponse.LastName);
            Assert.Single(doctorInResponse.HealthCenters);
            Assert.Equal(healthCenterId, doctorInResponse.HealthCenters.First().Id);
        }

        [Fact]
        public async Task GetDoctorList_WithFilters_ByHealthCenter_Successfully_Test()
        {
            // Arrange
            var (healthCenterId1, healthCenterId2, healthCenterId3, doctorId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var healthCentersOfFirstDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId1).Create(),
                Fixtures.GetHealthCenterComposer(healthCenterId2).Create()
            };
            var healthCentersOfSecondDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId3).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(doctorId, firstName: "Ameli").Create().SetHealthCenters(healthCentersOfFirstDoctor),
                Fixtures.GetDoctorComposer(Guid.NewGuid(), firstName: "Laura").Create().SetHealthCenters(healthCentersOfSecondDoctor),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await _context.AddRangeAsync(healthCentersOfFirstDoctor);
            await _context.AddRangeAsync(healthCentersOfSecondDoctor);
            await _context.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto { HealthCenterIds = new[] { healthCenterId2 } };
            var query = new GetDoctorListQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var response = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.Single(response.Items);
            var doctorInResponse = response.Items.ToArray()[0];
            Assert.Equal(doctorId, doctorInResponse.Id);
            Assert.Equal("Ameli", doctorInResponse.FirstName);
            Assert.Equal(2, doctorInResponse.HealthCenters.Count);
        }

        [Fact]
        public async Task GetDoctorList_WithFilters_ByHealthCenterAndName_Successfully_Test()
        {
            // Arrange
            var (healthCenterId1, healthCenterId2, healthCenterId3, doctorId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var healthCentersOfFirstDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId1).Create(),
                Fixtures.GetHealthCenterComposer(healthCenterId2).Create()
            };
            var healthCentersOfSecondDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId3).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(Guid.NewGuid(), firstName: "Ameli").Create().SetHealthCenters(healthCentersOfFirstDoctor),
                Fixtures.GetDoctorComposer(doctorId, firstName: "Laura").Create().SetHealthCenters(healthCentersOfSecondDoctor),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await _context.AddRangeAsync(healthCentersOfFirstDoctor);
            await _context.AddRangeAsync(healthCentersOfSecondDoctor);
            await _context.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto { Name = "Laura", HealthCenterIds = new[] { healthCenterId3 } };
            var query = new GetDoctorListQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var response = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.Single(response.Items);
            var doctorInResponse = response.Items.ToArray()[0];
            Assert.Equal(doctorId, doctorInResponse.Id);
            Assert.Equal("Laura", doctorInResponse.FirstName);
            Assert.Single(doctorInResponse.HealthCenters);
            Assert.Equal(healthCenterId3, doctorInResponse.HealthCenters.First().Id);
        }

        [Fact]
        public async Task GetDoctorList_WithFilters_ByHealthCenterAndName_NotFound_Test()
        {
            // Arrange
            var (healthCenterId1, healthCenterId2, healthCenterId3, doctorId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var healthCentersOfFirstDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId1).Create(),
                Fixtures.GetHealthCenterComposer(healthCenterId2).Create()
            };
            var healthCentersOfSecondDoctor = new List<HealthCenter>
            {
                Fixtures.GetHealthCenterComposer(healthCenterId3).Create()
            };
            var doctors = new List<Doctor>()
            {
                Fixtures.GetDoctorComposer(Guid.NewGuid(), firstName: "Ameli").Create().SetHealthCenters(healthCentersOfFirstDoctor),
                Fixtures.GetDoctorComposer(doctorId, firstName: "Laura").Create().SetHealthCenters(healthCentersOfSecondDoctor),
                Fixtures.GetDoctorComposer(Guid.NewGuid()).Create()
            };

            await _context.AddRangeAsync(healthCentersOfFirstDoctor);
            await _context.AddRangeAsync(healthCentersOfSecondDoctor);
            await _context.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();

            // Act
            var filterRequest = new DoctorListFilterRequestDto { Name = "Ameli", HealthCenterIds = new[] { healthCenterId3 } };
            var query = new GetDoctorListQuery(filterRequest, _defaultPagingRequest, SortingType.Asc);
            var response = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.Empty(response.Items);
        }

        private static Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }
    }
}
