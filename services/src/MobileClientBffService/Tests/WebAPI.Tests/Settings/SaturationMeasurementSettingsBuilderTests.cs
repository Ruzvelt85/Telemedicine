using System;
using Telemedicine.Services.MobileClientBffService.API.Settings;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Settings
{
    public class SaturationMeasurementSettingsBuilderTests
    {
        [Fact]
        public void BothLimitEqual_ShouldBeEqual()
        {
            // Arrange
            var optionsSnapshotMock = GetSaturationSettingsMockOptions(1234, 1234).Object;
            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(settings.MaxRawItemsToPassToMeasurementDSCountLimit, settings.MaxRawItemsValidationCountLimit);
        }

        [Fact]
        public void DbCountLimitGreatThenValidationLimit_ShouldBeDefaultValues()
        {
            var saturationMeasurementSettings = new SaturationMeasurementSettings();

            var validationLimit = saturationMeasurementSettings.MaxRawItemsValidationCountLimit;
            var dbLimit = saturationMeasurementSettings.MaxRawItemsToPassToMeasurementDSCountLimit;

            // Arrange
            var optionsSnapshotMock = GetSaturationSettingsMockOptions(validationLimit, dbLimit).Object;
            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
            Assert.Equal(dbLimit, settings.MaxRawItemsToPassToMeasurementDSCountLimit);
        }

        [Fact]
        public void DbCountLimitLessThenValidationLimit_ShouldBeAsIs()
        {
            const int validationLimit = 4321;
            const int dbLimit = 1234;

            // Arrange
            var optionsSnapshotMock = GetSaturationSettingsMockOptions(validationLimit, dbLimit).Object;
            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
            Assert.Equal(dbLimit, settings.MaxRawItemsToPassToMeasurementDSCountLimit);
        }

        [Fact]
        public void DbCountLimitLessThenZero_ShouldBeDefaultValues()
        {
            var saturationMeasurementSettings = new SaturationMeasurementSettings();

            var validationLimit = saturationMeasurementSettings.MaxRawItemsValidationCountLimit;
            var dbLimit = saturationMeasurementSettings.MaxRawItemsToPassToMeasurementDSCountLimit;

            // Arrange
            var optionsSnapshotMock = GetSaturationSettingsMockOptions(400, -100).Object;
            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
            Assert.Equal(dbLimit, settings.MaxRawItemsToPassToMeasurementDSCountLimit);
        }

        [Fact]
        public void OptionsValueThrowException_ShouldBeDefaultValues()
        {
            var saturationMeasurementSettings = new SaturationMeasurementSettings();

            var validationLimit = saturationMeasurementSettings.MaxRawItemsValidationCountLimit;
            var dbLimit = saturationMeasurementSettings.MaxRawItemsToPassToMeasurementDSCountLimit;

            // Arrange
            var optionsSnapshotMock = new Mock<IOptionsSnapshot<SaturationMeasurementSettings>>();
            optionsSnapshotMock.Setup(m => m.Value)
                .Throws<Exception>();

            var builder = new SaturationMeasurementSettingsBuilder(optionsSnapshotMock.Object);

            // Act
            var settings = builder.Build();

            // Assert
            Assert.Equal(validationLimit, settings.MaxRawItemsValidationCountLimit);
            Assert.Equal(dbLimit, settings.MaxRawItemsToPassToMeasurementDSCountLimit);
        }

        private static Mock<IOptionsSnapshot<SaturationMeasurementSettings>> GetSaturationSettingsMockOptions(int maxRawItemsValidationCountLimit,
            int maxRawItemsToPassToMeasurementDsCountLimit)
        {
            var mockOptions = new Mock<IOptionsSnapshot<SaturationMeasurementSettings>>();
            mockOptions.Setup(m => m.Value)
                .Returns(new SaturationMeasurementSettings
                {
                    MaxRawItemsValidationCountLimit = maxRawItemsValidationCountLimit,
                    MaxRawItemsToPassToMeasurementDSCountLimit = maxRawItemsToPassToMeasurementDsCountLimit
                });

            return mockOptions;
        }
    }
}
