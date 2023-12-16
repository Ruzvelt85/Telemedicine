using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Xunit;
using Moq;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.MobileClientBffService.API.Settings;
using Telemedicine.Services.MobileClientBffService.WebAPI.Commands;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.PulseRate;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Commands
{
    public class CreateSaturationMeasurementCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Guid _createdGuid;
        private readonly Guid _patientId;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ISaturationMeasurementSettingsBuilder _saturationMeasurementSettingsBuilder;

        public CreateSaturationMeasurementCommandHandlerTests()
        {
            _mapper = new MapperConfiguration(
                cfg => cfg.AddMaps(typeof(MobileClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
            _createdGuid = Guid.NewGuid();
            _patientId = Guid.NewGuid();

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(_patientId);
            _currentUserProvider = currentUserProviderMock.Object;
            _saturationMeasurementSettingsBuilder = GetSaturationMeasurementSettingsBuilderMock().Object;
        }

        [Fact]
        public async Task HandleAsync_SimpleCall_ShouldReturnGuid()
        {
            // Arrange
            var saturationMeasurementCommandServiceApiMock = new Mock<ISaturationMeasurementCommandService>();
            saturationMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(It.IsAny<CreateMeasurementRequestDto<SaturationMeasurementDto>>(), CancellationToken.None))
                .ReturnsAsync(_createdGuid);

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();

            var commandHandler = new CreateSaturationMeasurementCommandHandler(_mapper,
                _currentUserProvider,
                saturationMeasurementCommandServiceApiMock.Object,
                _saturationMeasurementSettingsBuilder,
                pulseRateMeasurementCommandServiceApiMock.Object);

            var command = new CreateSaturationMeasurementCommand();

            // Act
            var newItemId = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(_createdGuid, newItemId);
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public async Task HandleAsync_CheckServiceParam_ShouldBeCorrect(CreateSaturationMeasurementCommand command)
        {
            // Arrange
            var capturedServiceParams = new List<CreateMeasurementRequestDto<SaturationMeasurementDto>>();

            var saturationMeasurementCommandServiceApiMock = new Mock<ISaturationMeasurementCommandService>();
            saturationMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(Capture.In(capturedServiceParams), CancellationToken.None))
                .ReturnsAsync(_createdGuid);

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();

            var commandHandler = new CreateSaturationMeasurementCommandHandler(_mapper,
                _currentUserProvider,
                saturationMeasurementCommandServiceApiMock.Object,
                _saturationMeasurementSettingsBuilder,
                pulseRateMeasurementCommandServiceApiMock.Object);

            // Act
            var response = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(_createdGuid, response);
            Assert.Single(capturedServiceParams);
            Assert.Equal(_patientId, capturedServiceParams[0].PatientId);
            Assert.Equal(command.PulseRate, capturedServiceParams[0].Measure.PulseRate);
            Assert.Equal(command.Pi, capturedServiceParams[0].Measure.Pi);
            Assert.Equal(command.SpO2, capturedServiceParams[0].Measure.SpO2);

            CompareRawCollections(command.RawMeasurements, capturedServiceParams[0].Measure.RawMeasurements);

            Assert.Equal(command.ClientDate, capturedServiceParams[0].ClientDate);
        }


        [Theory]
        [InlineData(5, 4, 4)]
        [InlineData(5, 5, 5)]
        [InlineData(4, 5, 4)]
        [InlineData(4, null, null)]
        public async Task HandleAsync_CheckTrim_ShouldBeCorrect(int limitCount, int? inputCount, int? expectedCount)
        {
            // Arrange
            var capturedServiceParams = new List<CreateMeasurementRequestDto<SaturationMeasurementDto>>();

            var saturationMeasurementCommandServiceApiMock = new Mock<ISaturationMeasurementCommandService>();
            saturationMeasurementCommandServiceApiMock
                .Setup(m => m.CreateAsync(Capture.In(capturedServiceParams), CancellationToken.None))
                .ReturnsAsync(Guid.NewGuid());

            var saturationMeasurementSettingsBuilderMock = new Mock<ISaturationMeasurementSettingsBuilder>();
            saturationMeasurementSettingsBuilderMock
                .Setup(m => m.Build())
                .Returns(new SaturationMeasurementSettings { MaxRawItemsToPassToMeasurementDSCountLimit = limitCount });

            var pulseRateMeasurementCommandServiceApiMock = new Mock<IPulseRateMeasurementCommandService>();

            var command = new CreateSaturationMeasurementCommand
            {
                PulseRate = 1,
                Pi = 2,
                SpO2 = 3,
                ClientDate = DateTime.Now,
                RawMeasurements = inputCount is null
                ? null
                : Enumerable.Range(1, inputCount.Value).Select(x => new RawSaturationItemCommandDto { Order = x }).ToArray()
            };

            var commandHandler = new CreateSaturationMeasurementCommandHandler(_mapper,
                _currentUserProvider,
                saturationMeasurementCommandServiceApiMock.Object,
                saturationMeasurementSettingsBuilderMock.Object,
                pulseRateMeasurementCommandServiceApiMock.Object);

            // Act
            var response = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Single(capturedServiceParams);
            Assert.Equal(expectedCount, capturedServiceParams[0].Measure.RawMeasurements?.Count);
        }

        private static void CompareRawCollections(IReadOnlyCollection<RawSaturationItemCommandDto>? expectedCollection,
            IReadOnlyCollection<RawSaturationMeasurementItemDto>? actualCollection)
        {
            if (expectedCollection == null)
                return;

            var expectedCollectionArray = expectedCollection.ToArray();
            var actualCollectionArray = actualCollection!.ToArray();

            for (int i = 0; i < expectedCollection.Count; i++)
            {
                Assert.Equal(expectedCollectionArray[i].Order, actualCollectionArray[i].Order);
                Assert.Equal(expectedCollectionArray[i].PulseRate, actualCollectionArray[i].PulseRate);
                Assert.Equal(expectedCollectionArray[i].Pi, actualCollectionArray[i].Pi);
                Assert.Equal(expectedCollectionArray[i].SpO2, actualCollectionArray[i].SpO2);
                Assert.Equal(expectedCollectionArray[i].ClientDate, actualCollectionArray[i].ClientDate);
            }
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[]
                {
                    new CreateSaturationMeasurementCommand
                    {
                        PulseRate = 1,
                        Pi = 2,
                        SpO2 = 3,
                        ClientDate = DateTime.Now
                    }
                };
            yield return new object[]
                {
                    new CreateSaturationMeasurementCommand
                    {
                        PulseRate = 1,
                        Pi = 2,
                        SpO2 = 3,
                        ClientDate = DateTime.Now,
                        RawMeasurements = Array.Empty<RawSaturationItemCommandDto>()
                    }
                };
            yield return new object[]
            {
                new CreateSaturationMeasurementCommand
                {
                    PulseRate = 1,
                    Pi = 2,
                    SpO2 = 3,
                    ClientDate = DateTime.Now,
                    RawMeasurements = new List<RawSaturationItemCommandDto>
                    {
                        new()
                        {
                            Order = 1,
                            SpO2 = 2,
                            Pi = 3,
                            PulseRate = 4,
                            ClientDate = DateTime.Now
                        },
                        new()
                        {
                            Order = 2,
                            SpO2 = 6,
                            Pi = 7,
                            PulseRate = 8,
                            ClientDate = DateTime.Now
                        },
                        new()
                        {
                            Order = 1,
                            SpO2 = 6,
                            Pi = 7,
                            PulseRate = 8,
                            ClientDate = DateTime.Now
                        }
                    }
                }
            };
        }

        private Mock<ISaturationMeasurementSettingsBuilder> GetSaturationMeasurementSettingsBuilderMock()
        {
            var saturationMeasurementSettingsBuilder = new Mock<ISaturationMeasurementSettingsBuilder>();
            saturationMeasurementSettingsBuilder
                .Setup(m => m.Build())
                .Returns(new SaturationMeasurementSettings());
            return saturationMeasurementSettingsBuilder;
        }
    }
}
