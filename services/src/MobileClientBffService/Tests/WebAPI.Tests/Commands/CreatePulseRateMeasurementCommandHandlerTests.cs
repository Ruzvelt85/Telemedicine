using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Commands
{
    public class CreatePulseRateMeasurementCommandHandlerTests
    {
        private readonly IMapper _mapper;

        public CreatePulseRateMeasurementCommandHandlerTests()
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

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();
            pulseRateMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(It.IsAny<CreateMeasurementRequestDto<PulseRateMeasurementDto>>(), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(MobileClientBffService.WebAPI.Startup).Assembly)).CreateMapper();

            var commandHandler = new CreatePulseRateMeasurementCommandHandler(mapper,
                currentUserProviderMock.Object,
                pulseRateMeasurementCommandServiceApiMock.Object);

            var command = new CreatePulseRateMeasurementCommand();

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
            const int pulseRate = 60;

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(patientId);

            var capturedServiceParams = new List<CreateMeasurementRequestDto<PulseRateMeasurementDto>>();

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();
            pulseRateMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(Capture.In(capturedServiceParams), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var commandHandler = new CreatePulseRateMeasurementCommandHandler(_mapper,
                currentUserProviderMock.Object,
                pulseRateMeasurementCommandServiceApiMock.Object);

            var command = new CreatePulseRateMeasurementCommand
            {
                ClientDate = clientDate,
                PulseRate = pulseRate
            };

            // Act
            var response = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(createdGuid, response);
            Assert.Single(capturedServiceParams);
            Assert.Equal(patientId, capturedServiceParams[0].PatientId);
            Assert.Equal(clientDate, capturedServiceParams[0].ClientDate);
            Assert.Equal(pulseRate, capturedServiceParams[0].Measure.PulseRate);
        }
    }
}
