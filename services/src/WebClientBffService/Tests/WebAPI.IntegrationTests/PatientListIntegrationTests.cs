using System;
using System.Linq;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class PatientListIntegrationTests : IHttpServiceTests<IPatientQueryService>
    {
        private readonly IPatientQueryService _service;
        public HttpServiceFixture<IPatientQueryService> HttpServiceFixture { get; }

        public PatientListIntegrationTests(HttpServiceFixture<IPatientQueryService> httpServiceFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            _service = HttpServiceFixture.GetRestService();
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetPatientList_CommonSuccessfulIntegrationTest()
        {
            // For test purpose we use here existing ID from data seed
            // We cannot create new test data in databases due to need of isolation and other restrictions
            //Guid doctorId = Guid.Parse("AAB4ED21-0933-4D1F-ADD8-6F2CF2468789");

            var request = new PatientListRequestDto()
            {
                Paging = new PagingRequestDto(),
                Filter = new PatientListFilterRequestDto
                {
                    HealthCenterStructureFilter = HealthCenterStructureFilterType.InterdisciplinaryTeam
                }
            };

            PatientListResponseDto patientResponse = await _service.GetPatientListAsync(request);

            Assert.NotNull(patientResponse);
            Assert.NotNull(patientResponse.Paging);
            Assert.NotNull(patientResponse.Items);
            Assert.NotEmpty(patientResponse.Items!);

            var patientWithAppointments = patientResponse.Items!.FirstOrDefault(x =>
                x.PreviousAppointmentInfo != null && x.NextAppointmentInfo != null);

            Assert.NotNull(patientWithAppointments);
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetPatientById_ShouldReturnCorrectPatient()
        {
            // Arrange
            var patientId = Guid.Parse("84FC12F8-48B8-4A2C-80EE-0B9A9323841D");

            // Act
            var patientResponse = await _service.GetPatientByIdAsync(patientId);

            // Assert
            Assert.NotNull(patientResponse);
            Assert.Equal(patientId, patientResponse.Id);
            Assert.Equal("Vincent", patientResponse.FirstName);
            Assert.Equal("Black", patientResponse.LastName);
            Assert.Equal("+1-864-895-9735", patientResponse.PhoneNumber);
            Assert.Equal(new DateTime(1951, 11, 12), patientResponse.BirthDate);

            Assert.NotNull(patientResponse.HealthCenter);
            Assert.Equal(Guid.Parse("BB608923-866A-4008-A50F-B5042EE32AFB"), patientResponse.HealthCenter.Id);
            Assert.Equal("Pace Center 1", patientResponse.HealthCenter.Name);
            Assert.Equal("California", patientResponse.HealthCenter.State);

            Assert.NotNull(patientResponse.PrimaryCareProvider);
            Assert.Equal(Guid.Parse("0289D371-C4D5-4CC1-ABE2-42B0B9341A8B"), patientResponse.PrimaryCareProvider!.Id);
            Assert.Equal("Helen", patientResponse.PrimaryCareProvider.FirstName);
            Assert.Equal("Smith", patientResponse.PrimaryCareProvider.LastName);
        }
    }
}
