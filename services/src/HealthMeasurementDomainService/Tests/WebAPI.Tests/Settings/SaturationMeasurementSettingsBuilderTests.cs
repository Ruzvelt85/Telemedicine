using System;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Settings;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests.Settings
{
    public class SaturationMeasurementSettingsBuilderTests
    {
        [Fact]
        public void ValidationLimit_Correct_ShouldBeCorrect()
        {
            var validationLimit = 50;
            // Arrange
            var optionsSnapshotMock = GetSaturationSettingsMockOptions(validationLimit).Object;
            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void ValidationLimitLessOrEqualThenZero_ShouldBeDefaultValues(int limit)
        {
            var saturationMeasurementSettings = new SaturationMeasurementSettings();

            var validationLimit = saturationMeasurementSettings.MaxRawItemsValidationCountLimit;

            // Arrange
            var optionsSnapshotMock = GetSaturationSettingsMockOptions(limit).Object;
            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
        }

        [Fact]
        public void OptionsValueThrowException_ShouldBeDefaultValues()
        {
            var saturationMeasurementSettings = new SaturationMeasurementSettings();

            var validationLimit = saturationMeasurementSettings.MaxRawItemsValidationCountLimit;

            // Arrange
            var optionsSnapshotMock = new Mock<IOptionsSnapshot<SaturationMeasurementSettings>>();
            optionsSnapshotMock.Setup(m => m.Value)
                .Throws<Exception>();

            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock.Object);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
        }

        private static Mock<IOptionsSnapshot<SaturationMeasurementSettings>> GetSaturationSettingsMockOptions(int maxRawItemsValidationCountLimit)
        {
            var mockOptions = new Mock<IOptionsSnapshot<SaturationMeasurementSettings>>();
            mockOptions.Setup(m => m.Value)
                .Returns(new SaturationMeasurementSettings
                {
                    MaxRawItemsValidationCountLimit = maxRawItemsValidationCountLimit,
                });

            return mockOptions;
        }
    }
}
