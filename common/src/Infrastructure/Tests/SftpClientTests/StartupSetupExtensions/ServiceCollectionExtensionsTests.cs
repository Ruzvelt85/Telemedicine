using System;
using System.Threading.Tasks;
using FluentAssertions;
using Telemedicine.Common.Infrastructure.SftpClient;
using Telemedicine.Common.Infrastructure.SftpClient.StartupSetupExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Telemedicine.Common.Infrastructure.Tests.SftpClientTests.StartupSetupExtensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public async Task AddSftp_WhenProperlyConfigured_ShouldBeExpected()
        {
            // Arrange
            const string expectedName = "health_check_name";
            const HealthStatus expectedFailureStatus = HealthStatus.Degraded;
            var expectedTags = new[] { "tag1", "tag2" };
            var expectedTimeout = TimeSpan.FromSeconds(1);

            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddSftp(expectedName, expectedFailureStatus, expectedTags, expectedTimeout);

            await using var serviceProvider = services.BuildServiceProvider();
            var registration = Assert.Single(serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value.Registrations);

            // Act
            var check = registration.Factory(serviceProvider);

            // Assert
            registration.Name.Should().Be(expectedName);
            registration.FailureStatus.Should().Be(expectedFailureStatus);
            registration.Tags.IntersectWith(expectedTags);
            registration.Timeout.Should().Be(expectedTimeout);
            check.GetType().Should().Be(typeof(SftpHealthCheck));
        }
    }
}
