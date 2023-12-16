using FluentValidation.TestHelper;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Services.MobileClientBffService.API.HealthMeasurementCommandService.Dto;
using Telemedicine.Services.MobileClientBffService.API.Settings;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Validators
{
    public class CreateSaturationMeasurementRequestDtoValidatorTests
    {
        private readonly CreateSaturationMeasurementRequestDtoValidator _dtoValidator;

        public CreateSaturationMeasurementRequestDtoValidatorTests()
        {
            ISaturationMeasurementSettingsBuilder saturationSettingsBuilder = GetSaturationSettingsBuilderMock(100).Object;
            _dtoValidator = new CreateSaturationMeasurementRequestDtoValidator(saturationSettingsBuilder);
        }

        [Fact]
        public async Task AllProperties_Default_ShouldHaveValidationError()
        {
            // Arrange
            var model = new CreateSaturationMeasurementRequestDto();

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.SpO2);
            result.ShouldHaveValidationErrorFor(_ => _.Pi);
            result.ShouldHaveValidationErrorFor(_ => _.ClientDate);
            result.ShouldHaveAnyValidationError();
        }

        [Fact]
        public async Task Model_DefaultValuesInRawMeasurements_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var model = new CreateSaturationMeasurementRequestDto
            {
                SpO2 = 1,
                PulseRate = 1,
                Pi = 1,
                RawMeasurements = new List<RawSaturationItemRequestDto>
                {
                    new(),
                    new(),
                    new()
                },
                ClientDate = DateTime.UtcNow
            };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Model_EmptySaturationRawMeasurements_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var model = new CreateSaturationMeasurementRequestDto
            {
                SpO2 = 1,
                PulseRate = 1,
                Pi = 1,
                RawMeasurements = new List<RawSaturationItemRequestDto>(0),
                ClientDate = DateTime.Now
            };

            // Act
            var result = await _dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Model_MoreThenLimitRawMeasurements_ShouldHaveValidationErrors()
        {
            // Arrange
            ISaturationMeasurementSettingsBuilder saturationSettingsBuilder = GetSaturationSettingsBuilderMock(1).Object;
            var dtoValidator = new CreateSaturationMeasurementRequestDtoValidator(saturationSettingsBuilder);

            var model = GetCreateSaturationMeasurementRequestDto();

            // Act
            var result = await dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldHaveValidationErrorFor(_ => _.RawMeasurements);
        }

        [Fact]
        public async Task Model_Correct_ShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            ISaturationMeasurementSettingsBuilder saturationSettingsBuilder = GetSaturationSettingsBuilderMock(100).Object;
            var dtoValidator = new CreateSaturationMeasurementRequestDtoValidator(saturationSettingsBuilder);

            var model = GetCreateSaturationMeasurementRequestDto();

            // Act
            var result = await dtoValidator.TestValidateAsync(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        private CreateSaturationMeasurementRequestDto GetCreateSaturationMeasurementRequestDto()
        {
            return new CreateSaturationMeasurementRequestDto
            {
                SpO2 = 1,
                PulseRate = 1,
                Pi = 1,
                RawMeasurements = new List<RawSaturationItemRequestDto>
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
                        SpO2 = 2,
                        Pi = 3,
                        PulseRate = 4,
                        ClientDate = DateTime.Now
                    }
                },
                ClientDate = DateTime.Now
            };
        }

        private static Mock<ISaturationMeasurementSettingsBuilder> GetSaturationSettingsBuilderMock(int maxRawItemsValidationCountLimit)
        {
            var builderMock = new Mock<ISaturationMeasurementSettingsBuilder>();
            builderMock.Setup(m => m.Build())
                .Returns(new SaturationMeasurementSettings
                {
                    MaxRawItemsValidationCountLimit = maxRawItemsValidationCountLimit
                });

            return builderMock;
        }
    }
}
