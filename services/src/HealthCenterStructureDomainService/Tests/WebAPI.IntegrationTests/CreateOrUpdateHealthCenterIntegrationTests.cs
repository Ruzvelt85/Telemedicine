using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AutoFixture;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.HealthCentersCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class CreateOrUpdateHealthCenterIntegrationTests
        : IDisposable, IHttpServiceTests<IHealthCentersCommandService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IHealthCentersCommandService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IHealthCentersCommandService Service { get; }

        public CreateOrUpdateHealthCenterIntegrationTests(
            HttpServiceFixture<IHealthCentersCommandService> httpServiceFixture, EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task CreateHealthCenter_IntegrationTest_Successful()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer(innerId: "1010_PA", name: "Germantown").Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer(innerId: "1007_CO", name: "Loveland").Create();

            var healthCentersMock = new List<HealthCenter> { healthCenter1, healthCenter2 };

            await DbContext.AddRangeAsync(healthCentersMock);
            await DbContext.SaveChangesAsync();
            var initialCount = await DbContext.HealthCenters.CountAsync();

            var response = await Service.CreateOrUpdateAsync(new CreateOrUpdateHealthCenterRequestDto
            {
                InnerId = "1001_VA",
                Name = "Roanoke",
                UsaState = "VA",
                IsActive = true
            });

            // Assert
            Assert.NotEqual(Guid.Empty, response);
            Assert.Equal(initialCount + 1, await DbContext.HealthCenters.CountAsync());
            var savedHealthCenter = await DbContext.HealthCenters.FindAsync(response);
            Assert.NotNull(savedHealthCenter);
            Assert.Equal("Roanoke", savedHealthCenter.Name);
            Assert.Equal("1001_VA", savedHealthCenter.InnerId);
            Assert.Equal("VA", savedHealthCenter.UsaState);
            Assert.False(savedHealthCenter.IsDeleted);
        }

        [Fact]
        public async Task UpdateHealthCenter_IntegrationTest_Successful()
        {
            // Arrange
            var healthCenter1 = Fixtures.GetHealthCenterComposer(innerId: "1010_PA", name: "Germantown").Create();
            var healthCenter2 = Fixtures.GetHealthCenterComposer(innerId: "1007_CO", name: "Loveland", state: "CO").Create();

            var healthCentersMock = new List<HealthCenter> { healthCenter1, healthCenter2 };

            await DbContext.AddRangeAsync(healthCentersMock);
            await DbContext.SaveChangesAsync();
            DbContext.Entry(healthCenter2).State = EntityState.Detached;
            var initialCount = await DbContext.HealthCenters.CountAsync();

            var response = await Service.CreateOrUpdateAsync(new CreateOrUpdateHealthCenterRequestDto
            {
                InnerId = "1007_CO",
                Name = "Roanoke",
                UsaState = "VA",
                IsActive = true
            });

            // Assert
            Assert.NotEqual(Guid.Empty, response);

            Assert.Equal(initialCount, await DbContext.HealthCenters.CountAsync());
            var updatedHealthCenter = await DbContext.HealthCenters.FindAsync(response);
            Assert.NotNull(updatedHealthCenter);
            Assert.Equal("Roanoke", updatedHealthCenter.Name);
            Assert.Equal("1007_CO", updatedHealthCenter.InnerId);
            Assert.Equal("VA", updatedHealthCenter.UsaState);
            Assert.False(updatedHealthCenter.IsDeleted);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
