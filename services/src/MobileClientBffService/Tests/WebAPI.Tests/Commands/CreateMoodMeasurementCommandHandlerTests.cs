using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Xunit;
using Moq;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Commands
{
    public class CreateMoodMeasurementCommandHandlerTests
    {
        private readonly IMapper _mapper;

        public CreateMoodMeasurementCommandHandlerTests()
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

            var moodMeasurementCommandServiceApiMock = new Mock<IMoodMeasurementCommandService>();
            moodMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(It.IsAny<CreateMeasurementRequestDto<MoodMeasurementDto>>(), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var commandHandler = new CreateMoodMeasurementCommandHandler(_mapper,
                currentUserProviderMock.Object,
                moodMeasurementCommandServiceApiMock.Object);

            var command = new CreateMoodMeasurementCommand();

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
            var measure = API.MoodMeasureType.Happy;

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(patientId);

            var capturedServiceParams = new List<CreateMeasurementRequestDto<MoodMeasurementDto>>();

            var moodMeasurementCommandServiceApiMock = new Mock<IMoodMeasurementCommandService>();
            moodMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(Capture.In(capturedServiceParams), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var commandHandler = new CreateMoodMeasurementCommandHandler(_mapper,
                currentUserProviderMock.Object,
                moodMeasurementCommandServiceApiMock.Object);

            var command = new CreateMoodMeasurementCommand
            {
                ClientDate = clientDate,
                Measure = measure
            };

            // Act
            var response = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(createdGuid, response);
            Assert.Single(capturedServiceParams);
            Assert.Equal(patientId, capturedServiceParams[0].PatientId);
            Assert.Equal(clientDate, capturedServiceParams[0].ClientDate);
            Assert.Equal((int)measure, (int)capturedServiceParams[0].Measure.Measure);
        }
    }
}
