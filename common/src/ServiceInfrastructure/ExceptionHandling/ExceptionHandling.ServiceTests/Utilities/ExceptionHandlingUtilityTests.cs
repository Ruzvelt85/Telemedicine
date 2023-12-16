using System;
using System.Collections.Generic;
using System.Reflection;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Settings;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.ServiceTests.Setup;
using Xunit;

namespace Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.ServiceTests.Utilities
{
    public class ExceptionHandlingUtilityTests
    {
        private readonly TestException _testException;
        private const string? IncludeAll = null;
        private const string? IncludeAll2 = "   ";

        public ExceptionHandlingUtilityTests()
        {
            _testException = new TestException("test exception message", TestException.ErrorCode.TestError);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterThatHasAllParamsMatching_ShouldReturnTrue()
        {
            //Arrange
            var filterSettings = InfoLogExceptionFilterSettingsListBuilder.BuildWithOneDefault();
            InfoLogExceptionFilterSettings filter = filterSettings[0];

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterThatHasAllParamsMatchingButInDifferentCase_ShouldBeCaseInsensitiveAndReturnTrue()
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb("GeT")
                .WithPath("Api/aPPoinTMENts")
                .WithType(_testException.GetType().FullName!.ToUpper())
                .WithCode(TestException.ErrorCode.TestError.ToErrorCodeString().ToUpper() + "   ")
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenSettingsAreEmptyArray_ShouldReturnFalse()
        {
            //Arrange
            InfoLogExceptionFilterSettings[] filterSettings = Array.Empty<InfoLogExceptionFilterSettings>();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, "POST", "api/appointments", _testException);

            //Assert
            Assert.False(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterWithNullHttpVerb_ShouldReturnTrue()
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(null)
                .WithDefaultPath()
                .WithDefaultType()
                .WithDefaultCode()
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterWithNullPath_ShouldReturnTrue()
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithDefaultHttpVerb()
                .WithPath(null)
                .WithDefaultType()
                .WithDefaultCode()
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterWithNullType_ShouldReturnTrue()
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithDefaultHttpVerb()
                .WithDefaultPath()
                .WithType(null)
                .WithDefaultCode()
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterWithNullCode_ShouldReturnTrue()
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithDefaultHttpVerb()
                .WithDefaultPath()
                .WithDefaultType()
                .WithCode(null)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Fact]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterWithIncludeAllParams_ShouldReturnTrue()
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(IncludeAll)
                .WithPath(IncludeAll)
                .WithType(IncludeAll)
                .WithCode(IncludeAll)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(IncludeAll)]
        [InlineData(nameof(TestException.ErrorCode.TestError))]
        [InlineData("not_matching_code")]
        public void LogExceptionAsInfo_WhenCalledWithOneFilterWithIncludeAllParamsAndAnyCode_CodeMustBeIgnoredAndShouldReturnTrue(string? code)
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(IncludeAll)
                .WithPath(IncludeAll)
                .WithType(IncludeAll)
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.True(actualResult);
        }

        [Theory]
        [InlineData(IncludeAll, true)]
        [InlineData(IncludeAll2, true)]
        [InlineData(nameof(TestException.ErrorCode.TestError), true)]
        [InlineData("Not_existing_code", false)]
        [InlineData("Not_existing_code2", false)]
        public void LogExceptionAsInfo_WhenCalledWithHttpVerbAndPathSetToIncludeAllAndMatchingType_ShouldReturnExpectedResult(string code, bool expectedResult)
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(IncludeAll)
                .WithPath(IncludeAll)
                .WithType(_testException.GetType().FullName)
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(IncludeAll, false)]
        [InlineData(IncludeAll2, false)]
        [InlineData(nameof(TestException.ErrorCode.TestError), false)]
        [InlineData("Not_existing_code", false)]
        [InlineData("Not_existing_code2", false)]
        public void LogExceptionAsInfo_WhenCalledWithHttpVerbAndPathSetToIncludeAllAndNotMatchingType_ShouldReturnExpectedResult(string code, bool expectedResult)
        {
            //Arrange
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(IncludeAll)
                .WithPath(IncludeAll)
                .WithType("NotExistingType")
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, filter.Path!, _testException);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("api/test", true, IncludeAll, true)]
        [InlineData("api/test", true, IncludeAll2, true)]
        [InlineData("api/test", true, nameof(TestException.ErrorCode.TestError), true)]
        [InlineData("api/test", true, "Not_existing_code", false)]
        [InlineData("api/another-test", false, IncludeAll, false)]
        [InlineData("api/another-test", false, IncludeAll2, false)]
        [InlineData("api/another-test", false, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData("api/another-test", false, "Not_existing_code", false)]
        public void LogExceptionAsInfo_WhenCalledWithHttpVerbSetToIncludeAllAndMatchingPath_ShouldReturnExpectedResult(string path, bool isMatchingType, string? code, bool expectedResult)
        {
            //Arrange
            string type = isMatchingType ? _testException.GetType().FullName! : "not_existing_type";
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(IncludeAll)
                .WithPath(path)
                .WithType(type)
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, path, _testException);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("api/test", true, IncludeAll, false)]
        [InlineData("api/test", true, IncludeAll2, false)]
        [InlineData("api/test", true, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData("api/test", true, "Not_existing_code", false)]
        [InlineData("api/another-test", false, IncludeAll, false)]
        [InlineData("api/another-test", false, IncludeAll2, false)]
        [InlineData("api/another-test", false, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData("api/another-test", false, "Not_existing_code", false)]
        public void LogExceptionAsInfo_WhenCalledWithHttpVerbSetToIncludeAllAndNotMatchingPath_ShouldReturnExpectedResult(string path, bool isMatchingType, string? code, bool expectedResult)
        {
            //Arrange
            string type = isMatchingType ? _testException.GetType().FullName! : "not_existing_type";
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb(IncludeAll)
                .WithPath(path)
                .WithType(type)
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, "api/appointments", _testException);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(true, true, IncludeAll, true)]
        [InlineData(true, true, IncludeAll2, true)]
        [InlineData(true, true, nameof(TestException.ErrorCode.TestError), true)]
        [InlineData(true, true, "Not_existing_code", false)]
        [InlineData(true, false, IncludeAll, false)]
        [InlineData(true, false, IncludeAll2, false)]
        [InlineData(true, false, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(true, false, "Not_existing_code", false)]
        [InlineData(false, true, IncludeAll, false)]
        [InlineData(false, true, IncludeAll2, false)]
        [InlineData(false, true, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(false, true, "Not_existing_code", false)]
        [InlineData(false, false, IncludeAll, false)]
        [InlineData(false, false, IncludeAll2, false)]
        [InlineData(false, false, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(false, false, "Not_existing_code", false)]
        public void LogExceptionAsInfo_WhenCalledWithMatchingHttpVerb_ShouldReturnExpectedResult(bool isMatchingPath, bool isMatchingType, string? code, bool expectedResult)
        {
            //Arrange
            string path = "api/appointments";
            string type = isMatchingType ? _testException.GetType().FullName! : "not_existing_type";
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb("GET")
                .WithPath(path)
                .WithType(type)
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, filter.HttpVerb!, isMatchingPath ? path : path + "test", _testException);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(true, true, IncludeAll, false)]
        [InlineData(true, true, IncludeAll2, false)]
        [InlineData(true, true, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(true, true, "Not_existing_code", false)]
        [InlineData(true, false, IncludeAll, false)]
        [InlineData(true, false, IncludeAll2, false)]
        [InlineData(true, false, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(true, false, "Not_existing_code", false)]
        [InlineData(false, true, IncludeAll, false)]
        [InlineData(false, true, IncludeAll2, false)]
        [InlineData(false, true, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(false, true, "Not_existing_code", false)]
        [InlineData(false, false, IncludeAll, false)]
        [InlineData(false, false, IncludeAll2, false)]
        [InlineData(false, false, nameof(TestException.ErrorCode.TestError), false)]
        [InlineData(false, false, "Not_existing_code", false)]
        public void LogExceptionAsInfo_WhenCalledWithNotMatchingHttpVerb_ShouldReturnExpectedResult(bool isMatchingPath, bool isMatchingType, string? code, bool expectedResult)
        {
            //Arrange
            string path = "api/appointments";
            string type = isMatchingType ? _testException.GetType().FullName! : "not_existing_type";
            var filter = new InfoLogExceptionFilterSettingsBuilder()
                .WithHttpVerb("GET")
                .WithPath(path)
                .WithType(type)
                .WithCode(code)
                .Build();
            var filterSettings = new InfoLogExceptionFilterSettingsListBuilder().AddOne(filter).Build();

            //Act
            var actualResult = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, "POST", isMatchingPath ? path : path + "test", _testException);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }
    }

    internal class InfoLogExceptionFilterSettingsListBuilder
    {
        private readonly List<InfoLogExceptionFilterSettings> _settings = new();

        public IReadOnlyCollection<InfoLogExceptionFilterSettings> Build()
        {
            return _settings;
        }

        public InfoLogExceptionFilterSettingsListBuilder AddOneDefault()
        {
            _settings.Add(new InfoLogExceptionFilterSettingsBuilder().WithDefault().Build());

            return this;
        }

        public InfoLogExceptionFilterSettingsListBuilder AddOne(InfoLogExceptionFilterSettings setting)
        {
            _settings.Add(setting);

            return this;
        }


        public static List<InfoLogExceptionFilterSettings> BuildWithOneDefault()
        {
            return new List<InfoLogExceptionFilterSettings>
                {
                    new InfoLogExceptionFilterSettingsBuilder().WithDefault().Build()
                };
        }
    }

    internal class InfoLogExceptionFilterSettingsBuilder
    {
        private readonly InfoLogExceptionFilterSettings _setting = new();

        public InfoLogExceptionFilterSettings Build() => _setting;

        public InfoLogExceptionFilterSettingsBuilder WithDefault()
        {
            return this.WithDefaultHttpVerb().WithDefaultPath().WithDefaultType().WithDefaultCode();
        }

        public InfoLogExceptionFilterSettingsBuilder WithPath(string? path)
        {
            PropertyInfo pathProperty = _setting.GetType().GetProperty(nameof(_setting.Path))!;
            pathProperty.SetValue(_setting, path);

            return this;
        }

        public InfoLogExceptionFilterSettingsBuilder WithDefaultPath()
        {
            return WithPath("api/appointments");
        }

        public InfoLogExceptionFilterSettingsBuilder WithHttpVerb(string? httpVerb)
        {
            PropertyInfo pathProperty = _setting.GetType().GetProperty(nameof(_setting.HttpVerb))!;
            pathProperty.SetValue(_setting, httpVerb);

            return this;
        }

        public InfoLogExceptionFilterSettingsBuilder WithDefaultHttpVerb()
        {
            return WithHttpVerb("GET");
        }

        public InfoLogExceptionFilterSettingsBuilder WithType(string? type)
        {
            PropertyInfo pathProperty = _setting.GetType().GetProperty(nameof(_setting.Type))!;
            pathProperty.SetValue(_setting, type);

            return this;
        }

        public InfoLogExceptionFilterSettingsBuilder WithDefaultType()
        {
            return WithType(typeof(TestException).FullName!);
        }

        public InfoLogExceptionFilterSettingsBuilder WithCode(string? code)
        {
            PropertyInfo pathProperty = _setting.GetType().GetProperty(nameof(_setting.Code))!;
            pathProperty.SetValue(_setting, code);

            return this;
        }

        public InfoLogExceptionFilterSettingsBuilder WithDefaultCode()
        {
            return WithCode(TestException.ErrorCode.TestError.ToErrorCodeString());
        }
    }
}
