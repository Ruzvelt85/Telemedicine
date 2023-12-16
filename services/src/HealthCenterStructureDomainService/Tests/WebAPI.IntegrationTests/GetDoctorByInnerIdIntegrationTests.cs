using Refit;
using Xunit;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;
using Telemedicine.Services.HealthCenterStructureDomainService.Tests.Core;

namespace Telemedicine.Services.HealthCenterStructureDomainService.Tests.WebAPI.IntegrationTests
{
    [Collection("Integration")]
    [Trait("Category", "Integration")]
    public sealed class GetDoctorByInnerIdIntegrationTests
        : IDisposable, IHttpServiceTests<IDoctorsQueryService>, IDbContextTests<HealthCenterStructureDomainServiceDbContext>
    {
        public HttpServiceFixture<IDoctorsQueryService> HttpServiceFixture { get; }
        public EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> EfCoreContextFixture { get; }
        public HealthCenterStructureDomainServiceDbContext DbContext { get; }
        public IDoctorsQueryService Service { get; }

        public GetDoctorByInnerIdIntegrationTests(
            HttpServiceFixture<IDoctorsQueryService> httpServiceFixture, EfCoreContextFixture<HealthCenterStructureDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            DbContext = EfCoreContextFixture.DbContext;
            Service = HttpServiceFixture.GetRestService();
        }

        [Fact]
        public async Task GetDoctor_IntegrationTest_Successful()
        {
            const string innerId = "123";
            var id = Guid.NewGuid();
            var doctorsMock = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(innerId: innerId, id: id).Create(),
                Fixtures.GetDoctorComposer(innerId: "456").Create(),
                Fixtures.GetDoctorComposer(innerId: "789").Create()
            };

            await DbContext.AddRangeAsync(doctorsMock);
            await DbContext.SaveChangesAsync();

            var request = new DoctorByInnerIdRequestDto(innerId);

            var response = await Service.GetDoctorByInnerIdAsync(request);

            Assert.NotNull(response);
            Assert.Equal(id, response.Id);
        }

        [Fact]
        public async Task GetDoctorIdByInnerId_IntegrationTest_NotFound()
        {
            const string innerId = "012";
            var doctorsMock = new List<Doctor>
            {
                Fixtures.GetDoctorComposer(innerId: "123").Create(),
                Fixtures.GetDoctorComposer(innerId: "456").Create(),
                Fixtures.GetDoctorComposer(innerId: "789").Create()
            };
            await DbContext.AddRangeAsync(doctorsMock);
            await DbContext.SaveChangesAsync();

            var request = new DoctorByInnerIdRequestDto(innerId);

            var exception = await Record.ExceptionAsync(() => Service.GetDoctorByInnerIdAsync(request));

            Assert.NotNull(exception);
            Assert.IsType<ApiException>(exception);
        }

        public void Dispose()
        {
            EfCoreContextFixture.CleanAllTablesExcept().Wait();
        }
    }
}
