using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientById;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.Patients
{
    public class GetPatientByIdQueryHandlerTests
    {
        private readonly GetPatientByIdQueryHandler _queryHandler;
        private readonly Mock<IPatientsQueryService> _healthCenterStructureServiceApiMock;

        public GetPatientByIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
            _healthCenterStructureServiceApiMock = new Mock<IPatientsQueryService>();

            _queryHandler = new GetPatientByIdQueryHandler(mapper, _healthCenterStructureServiceApiMock.Object);
        }

        [Fact]
        public async Task HandleAsync_GetPatientById_ShouldReturnPatientResponse()
        {
            //Arrange
            var patientResponseDto = new PatientByIdResponseDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Black",
                PhoneNumber = "123-456-78",
                BirthDate = new DateTime(1950, 6, 1),
                HealthCenter = new HealthCenterResponseDto { Id = Guid.NewGuid(), Name = "Health Center for test", UsaState = "California" },
                PrimaryCareProvider = new PrimaryCareProviderResponseDto { Id = Guid.NewGuid(), LastName = "Samantha", FirstName = "Brown" }
            };

            _healthCenterStructureServiceApiMock
                .Setup(_ => _.GetPatientByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(patientResponseDto);

            var query = new GetPatientByIdQuery(Guid.NewGuid());

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientResponseDto.Id, result.Id);
            Assert.Equal(patientResponseDto.FirstName, result.FirstName);
            Assert.Equal(patientResponseDto.LastName, result.LastName);
            Assert.Equal(patientResponseDto.PhoneNumber, result.PhoneNumber);
            Assert.Equal(patientResponseDto.BirthDate, result.BirthDate);

            Assert.NotNull(result.HealthCenter);
            Assert.Equal(patientResponseDto.HealthCenter.Id, result.HealthCenter.Id);
            Assert.Equal(patientResponseDto.HealthCenter.Name, result.HealthCenter.Name);
            Assert.Equal(patientResponseDto.HealthCenter.UsaState, result.HealthCenter.State);

            Assert.NotNull(result.PrimaryCareProvider);
            Assert.Equal(patientResponseDto.PrimaryCareProvider!.Id, result.PrimaryCareProvider!.Id);
            Assert.Equal(patientResponseDto.PrimaryCareProvider.FirstName, result.PrimaryCareProvider.FirstName);
            Assert.Equal(patientResponseDto.PrimaryCareProvider.LastName, result.PrimaryCareProvider.LastName);
        }

        [Fact]
        public async Task HandleAsync_GetPatientByIdWithAbsentPcpProvider_ShouldReturnPatientResponse()
        {
            //Arrange
            var patientResponseDto = new PatientByIdResponseDto
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Black",
                PhoneNumber = "123-456-78",
                BirthDate = new DateTime(1950, 6, 1),
                HealthCenter = new HealthCenterResponseDto { Id = Guid.NewGuid(), Name = "Health Center for test", UsaState = "California" },
                PrimaryCareProvider = null
            };

            _healthCenterStructureServiceApiMock
                .Setup(_ => _.GetPatientByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(patientResponseDto);

            var query = new GetPatientByIdQuery(Guid.NewGuid());

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientResponseDto.Id, result.Id);
            Assert.Equal(patientResponseDto.FirstName, result.FirstName);
            Assert.Equal(patientResponseDto.LastName, result.LastName);
            Assert.Equal(patientResponseDto.PhoneNumber, result.PhoneNumber);
            Assert.Equal(patientResponseDto.BirthDate, result.BirthDate);

            Assert.NotNull(result.HealthCenter);
            Assert.Equal(patientResponseDto.HealthCenter.Id, result.HealthCenter.Id);
            Assert.Equal(patientResponseDto.HealthCenter.Name, result.HealthCenter.Name);
            Assert.Equal(patientResponseDto.HealthCenter.UsaState, result.HealthCenter.State);

            Assert.Null(result.PrimaryCareProvider);
        }
    }
}
