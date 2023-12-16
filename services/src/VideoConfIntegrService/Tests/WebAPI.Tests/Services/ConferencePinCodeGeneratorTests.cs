using System.Collections.Generic;
using Xunit;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Services;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests.Services
{
    public class ConferencePinCodeGeneratorTests
    {
        private static IEnumerable<object[]> GetPinCodeData()
        {
            yield return new object[]
            {
                "Pin code is 6 digits",
                6, 6
            };
            yield return new object[]
            {
                "Pin code is 12 digits",
                12, 12
            };
            yield return new object[]
            {
                "Digit quantity of pin code is -1 (less than 6 or more than 12, should return 10 digits pin code)",
                -1, 10
            };
            yield return new object[]
            {
                "Digit quantity of pin code is 3 (less than 6 or more than 12, should return 10 digits pin code)",
                3, 10
            };
            yield return new object[]
            {
                "Digit quantity of pin code is 3 (less than 6 or more than 12, should return 10 digits pin code)",
                100, 10
            };
        }

        [Theory]
        [MemberData(nameof(GetPinCodeData))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public void Tests(string msg, int pinCodeDigitQuantity, int expectedLength)
        {
            var actualResult = ConferencePinCodeGenerator.Generate(pinCodeDigitQuantity);
            Assert.Equal(expectedLength, actualResult.Length);
        }
    }
}