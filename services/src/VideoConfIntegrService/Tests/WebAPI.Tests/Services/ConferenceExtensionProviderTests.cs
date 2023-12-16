using System;
using Xunit;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Services;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.WebAPI.Tests.Services
{
    public class ConferenceExtensionProviderTests
    {

        [Fact]
        public void TestGenerationExtensionWithSuccess()
        {
            var correctExtensionPrefixes = new[] { "1", "12345678910" };

            foreach (var prefix in correctExtensionPrefixes)
            {
                var result = ConferenceExtensionProvider.GenerateExtension(prefix);
                Assert.NotNull(result);
                Assert.StartsWith(prefix, result);
                Assert.NotEqual(prefix, result);
            }
        }

        [Fact]
        public void TestGenerationExtensionWithException()
        {
            var incorrectExtensionPrefixes = new[] { null, "", " ", "Abc", "40A", "-40", "0" };

            foreach (var prefix in incorrectExtensionPrefixes)
            {
                Assert.Throws<ArgumentException>(() => ConferenceExtensionProvider.GenerateExtension(prefix));
            }
        }
    }
}
