using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.BloodPressure.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Commands
{
    public class CreateBloodPressureMeasurementCommandHandlerTests
    {
        private readonly IMapper _mapper;

        public CreateBloodPressureMeasurementCommandHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(MobileClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
        }

        [Fact]
        public async Task HandleAsync_SimpleCall_ShouldReturnGuid()
        {
            // Arrange
            var createdGuid = Guid.NewGuid();

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(Guid.NewGuid());

            var bloodPressureMeasurementCommandServiceApiMock = new Mock<IBloodPressureMeasurementCommandService>();
            bloodPressureMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(It.IsAny<CreateMeasurementRequestDto<BloodPressureMeasurementDto>>(), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();

            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(MobileClientBffService.WebAPI.Startup).Assembly)).CreateMapper();

            var commandHandler = new CreateBloodPressureMeasurementCommandHandler(mapper,
                currentUserProviderMock.Object,
                bloodPressureMeasurementCommandServiceApiMock.Object,
                pulseRateMeasurementCommandServiceApiMock.Object);

            var command = new CreateBloodPressureMeasurementCommand();

            // Act
            var newItemId = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(createdGuid, newItemId);
        }

        [Fact]
        public async Task HandleAsync_CheckServiceParam_ShouldBeCorrect()
        {
            // Arrange
            var createdGuid = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var clientDate = DateTime.Now;
            const int systolic = 120;
            const int diastolic = 70;
            const int pulseRate = 60;

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(patientId);

            var capturedServiceParams = new List<CreateMeasurementRequestDto<BloodPressureMeasurementDto>>();

            var bloodPressureMeasurementCommandServiceApiMock = new Mock<IBloodPressureMeasurementCommandService>();
            bloodPressureMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(Capture.In(capturedServiceParams), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();

            var commandHandler = new CreateBloodPressureMeasurementCommandHandler(_mapper,
                currentUserProviderMock.Object,
                bloodPressureMeasurementCommandServiceApiMock.Object,
                pulseRateMeasurementCommandServiceApiMock.Object);

            var command = new CreateBloodPressureMeasurementCommand
            {
                ClientDate = clientDate,
                Systolic = systolic,
                Diastolic = diastolic,
                PulseRate = pulseRate
            };

            // Act
            var response = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(createdGuid, response);
            Assert.Single(capturedServiceParams);
            Assert.Equal(patientId, capturedServiceParams[0].PatientId);
            Assert.Equal(clientDate, capturedServiceParams[0].ClientDate);
            Assert.Equal(systolic, capturedServiceParams[0].Measure.Systolic);
            Assert.Equal(diastolic, capturedServiceParams[0].Measure.Diastolic);
            Assert.Equal(pulseRate, capturedServiceParams[0].Measure.PulseRate);
        }
    }
}
