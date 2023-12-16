using System;
using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.Tests.GlobalContracts.ExceptionTypes;
using Telemedicine.Common.Infrastructure.Testing;
using Xunit;

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts
{
    public class ExceptionTests : UnitTestsBase
    {
        private const string DefaultTestExceptionMessage = "Test exception message.";
        private static readonly SerializableType _serializableType = new() { StringProperty = "StringValue2" };
        private static readonly SerializableTypeWithNonSerializableProperty _serializableType2 = new() { TypeProperty = typeof(ExceptionTests) };

        private static readonly Dictionary<string, (object? value, bool expected)> _propertiesDictionary = new()
        {
            { "TypeProperty", (typeof(ExceptionTests), false) },
            { "NonSerializableTypeProperty", (new NonSerializableType() { StringProperty = "StringValue3" }, false) },
            { "StringProperty", ("StringValue1", true) },
            { "ByteProperty", ((byte)123, true) },
            { "LongProperty", ((long)123456, true) },
            { "BoolProperty", (false, true) },
            { "DateTimeProperty", (DateTime.Now, true) },
            { "TimeSpanProperty", (TimeSpan.FromMinutes(25), true) },
            { "TimeStampProperty", (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(), true) },
            { "GuidProperty", (Guid.NewGuid(), true) },
            { "ByteArrayProperty", (new byte[] { 12, 23, 34, 45, 56 }, true) },
            { "IntArrayProperty", (new[] { 1234, 2345, 3456, 4567, 5678 }, true) },
            { "NullProperty", (null, true) },
            { "SerializableTypeProperty", (_serializableType, true) },
            { "SerializableTypeWithNonSerializableProperty", (_serializableType2, true) },
        };

        public static IEnumerable<object?[]> GetTestData()
        {
            foreach (var (key, (value, expected)) in _propertiesDictionary)
            {
                yield return new[] { key, value, expected };
            }
        }

        public static IEnumerable<object?[]> GetTestData2()
        {
            foreach (var (key, (value, expected)) in _propertiesDictionary)
            {
                var exception = new TestException(DefaultTestExceptionMessage, TestException.ErrorCode.TestError);
                var exceptionType = exception.GetType();
                var propertyInfo = exceptionType.GetProperty(key);
                if (propertyInfo == null)
                {
                    throw new ArgumentNullException($"Property with name '{key}' is not found. Please, add property to TestException class.");
                }

                propertyInfo.SetValue(exception, value);
                yield return new[] { key, value, exception, expected };
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void AddData_CustomException_Test(string propertyName, object value, bool expected)
        {
            var dict = new Dictionary<string, object?>
            {
                { propertyName, value },
            };
#pragma warning disable 618
            void ThrowException() => throw new TestException(DefaultTestExceptionMessage, dict);
#pragma warning restore 618

            var ex = Record.Exception(ThrowException);

            Assert.NotNull(ex);
            Assert.IsType<TestException>(ex);
            Assert.Equal(expected ? 2 : 1, ex?.Data.Count);
            Assert.True(ex?.Data.Contains("Code"));
            Assert.Equal("EmptyCode", ex?.Data["Code"]);
            if (expected)
            {
                Assert.Equal(expected, ex?.Data.Contains(propertyName));
                Assert.Equal(value, ex?.Data[propertyName]);
                var actualPropertyValue = GetPropertyValue(ex, propertyName);
                Assert.Equal(value, actualPropertyValue);
            }
        }

        [Theory]
        [MemberData(nameof(GetTestData2))]
        public void SetProperty_CustomException_Test(string propertyName, object value, Exception exception, bool expected)
        {
            void ThrowException()
            {
                throw exception;
            }

            var ex = Record.Exception(ThrowException);

            Assert.NotNull(ex);
            Assert.IsType<TestException>(ex);
            Assert.Equal(expected ? 2 : 1, ex?.Data.Count);
            Assert.True(ex?.Data.Contains("Code"));
            Assert.Equal("TestError", ex?.Data["Code"]);
            if (expected)
            {
                Assert.Equal(expected, ex?.Data.Contains(propertyName));
                Assert.Equal(value, ex?.Data[propertyName]);
                var actualPropertyValue = GetPropertyValue(ex, propertyName);
                Assert.Equal(value, actualPropertyValue);
            }
        }

        [Fact]
        public void ReplaceProperty_CustomException_Test()
        {
            const string newTestCode = "NewTestCode";
            static void ThrowException()
            {
                var exception = new TestException(DefaultTestExceptionMessage, TestException.ErrorCode.TestError)
                {
                    Code = newTestCode
                };
                throw exception;
            }

            var ex = Record.Exception(ThrowException);

            Assert.NotNull(ex);
            Assert.IsType<TestException>(ex);
            Assert.Equal(1, ex?.Data.Count);
            Assert.True(ex?.Data.Contains("Code"));
            Assert.Equal(newTestCode, ex?.Data["Code"]);
            Assert.Equal(newTestCode, ((ServiceLayerException)ex!).Code);
        }

        [Fact]
        public void EntityNotFoundByIdException_Test()
        {
            var id = Guid.NewGuid();
            var type = typeof(ExceptionTests);
            void ThrowException() => throw new EntityNotFoundByIdException(type, id);

            var ex = Record.Exception(ThrowException);

            Assert.NotNull(ex);
            Assert.IsType<EntityNotFoundByIdException>(ex);
            Assert.Equal(3, ex?.Data.Count);
            Assert.True(ex?.Data.Contains("Code"));
            Assert.True(ex?.Data.Contains("Type"));
            Assert.True(ex?.Data.Contains("Id"));

            Assert.Equal("EntityNotFound", ex?.Data["Code"]);
            Assert.Equal(type.FullName, ex?.Data["Type"]);
            Assert.Equal(id, ex?.Data["Id"]);

            Assert.Equal($"Entity with type '{type.FullName}' and id '{id}' can not be found.", ex?.Message);
        }

        [Fact]
        public void EntityAlreadyDeletedException_Test()
        {
            var id = Guid.NewGuid();
            var type = typeof(ExceptionTests);
            void ThrowException() => throw new EntityAlreadyDeletedException(type, id);

            var ex = Record.Exception(ThrowException);

            Assert.NotNull(ex);
            Assert.IsType<EntityAlreadyDeletedException>(ex);
            Assert.Equal(3, ex?.Data.Count);
            Assert.True(ex?.Data.Contains("Code"));
            Assert.True(ex?.Data.Contains("Type"));
            Assert.True(ex?.Data.Contains("Id"));

            Assert.Equal("EntityAlreadyDeleted", ex?.Data["Code"]);
            Assert.Equal(type.FullName, ex?.Data["Type"]);
            Assert.Equal(id, ex?.Data["Id"]);

            Assert.Equal($"Entity with type '{type.FullName}' and id '{id}' has already been deleted.", ex?.Message);
        }

        private static object? GetPropertyValue(Exception? ex, string propertyName)
        {
            var testException = ex as TestException;
            var exceptionType = testException?.GetType();
            var propertyInfo = exceptionType?.GetProperty(propertyName);
            return propertyInfo?.GetValue(testException);
        }
    }
}
