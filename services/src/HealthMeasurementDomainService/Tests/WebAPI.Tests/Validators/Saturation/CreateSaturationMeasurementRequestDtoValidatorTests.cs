using FluentValidation.TestHelper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Saturation.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Validators.Saturation
{
    public class CreateSaturationMeasurementRequestDtoValidatorTests
    {
        private readonly CreateMeasurementRequestDto<SaturationMeasurementDto> _dtoModel;
        private readonly CreateSaturationMeasurementRequestDtoValidator _dtoValidator;

        public CreateSaturationMeasurementRequestDtoValidatorTests()
        {
            var builderMock = GetSaturationMeasurementSettingsBuilderMock(int.MaxValue).Object;
            _dtoModel = new CreateMeasurementRequestDto<SaturationMeasurementDto>();
            _dtoValidator = new CreateSaturationMeasurementRequestDtoValidator(builderMock);
        }

        [Fact]
        public async Task SingleProperty_PatientId_ShouldHaveValidationError()
        {
            // Act
            var result = await _dtoValidator.TestValidateAsync(_dtoModel);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.PatientId);
        }

        [Fact]
        public async Task SingleProperty_PatientId_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { PatientId = Guid.NewGuid() };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.PatientId);
        }

        [Fact]
        public async Task SingleProperty_ClientDate_Default_ShouldHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { ClientDate = default };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
        }

        [Fact]
        public async Task SingleProperty_ClientDate_AboveZero_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { ClientDate = DateTime.Now };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.ClientDate);
        }

        [Fact]
        public async Task SingleProperty_PulseRate_Empty_ShouldHaveValidationError()
        {
            // Act
            var result = await _dtoValidator.TestValidateAsync(_dtoModel);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.Measure.PulseRate);
        }

        [Fact]
        public async Task SingleProperty_PulseRate_BelowZero_ShouldHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { PulseRate = -100 } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Measure.PulseRate);
        }

        [Fact]
        public async Task SingleProperty_PulseRate_AboveZero_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { PulseRate = 100 } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.Measure.PulseRate);
        }

        [Fact]
        public async Task SingleProperty_PulseRate_MoreThan255_ShouldHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { PulseRate = 256 } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Measure.PulseRate);
        }

        [Fact]
        public async Task SingleProperty_SpO2_Empty_ShouldHaveValidationError()
        {
            // Arrange & Act
            var result = await _dtoValidator.TestValidateAsync(_dtoModel);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Measure.SpO2);
        }

        [Fact]
        public async Task SingleProperty_SpO2_BelowZero_ShouldHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { SpO2 = -100 } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Measure.SpO2);
        }

        [Fact]
        public async Task SingleProperty_SpO2_AboveZero_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { SpO2 = 100 } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.Measure.SpO2);
        }

        [Fact]
        public async Task SingleProperty_Pi_Empty_ShouldHaveValidationError()
        {
            // Act && Arrange
            var result = await _dtoValidator.TestValidateAsync(_dtoModel);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Measure.Pi);
        }

        [Fact]
        public async Task SingleProperty_Pi_BelowZero_ShouldHaveValidationError()
        {
            // Act && Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { Pi = -100.1m } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.Measure.Pi);
        }

        [Fact]
        public async Task SingleProperty_Pi_AboveZero_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { Pi = 100.1m } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.Measure.Pi);
        }

        [Fact]
        public async Task SingleProperty_RawMeasurements_Default_ShouldNotHaveValidationError()
        {
            // Act
            var result = await _dtoValidator.TestValidateAsync(_dtoModel);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.Measure.RawMeasurements);
        }

        [Fact]
        public async Task SingleProperty_RawMeasurements_Empty_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = _dtoModel with { Measure = _dtoModel.Measure with { RawMeasurements = Array.Empty<RawSaturationMeasurementItemDto>() } };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(_ => _.Measure.RawMeasurements);
        }

        [Theory]
        [InlineData(1, 2, true)]
        [InlineData(2, 1, false)]
        [InlineData(2, 2, false)]
        public async Task SingleProperty_RawMeasurements_MaxQuantityLimit(int limit, int rawMeasurementQnt, bool shouldHaveValidationError)
        {
            // Arrange
            var optionsMonitorMock = GetSaturationMeasurementSettingsBuilderMock(limit).Object;
            var dtoValidator = new CreateSaturationMeasurementRequestDtoValidator(optionsMonitorMock);

            var rawMeasurements = Enumerable.Range(1, rawMeasurementQnt)
                .Select(x => new RawSaturationMeasurementItemDto
                {
                    Order = x,
                }).ToList();

            var model = _dtoModel with
            {
                Measure = _dtoModel.Measure with
                {
                    RawMeasurements = rawMeasurements
                }
            };

            // Act
            var result = await dtoValidator.TestValidateAsync(model);

            // Assert
            if (shouldHaveValidationError)
            {
                result.ShouldHaveValidationErrorFor(_ => _.Measure.RawMeasurements);
            }
            else
            {
                result.ShouldNotHaveValidationErrorFor(_ => _.Measure.RawMeasurements);
            }
        }

        private static Mock<ISaturationMeasurementSettingsBuilder> GetSaturationMeasurementSettingsBuilderMock(int maxRawItemsValidationCountLimit)
        {
            var mockBuilder = new Mock<ISaturationMeasurementSettingsBuilder>();
            mockBuilder.Setup(m => m.Build())
                .Returns(new SaturationMeasurementSettings
                {
                    MaxRawItemsValidationCountLimit = maxRawItemsValidationCountLimit
                });

            return mockBuilder;
        }
    }
}
