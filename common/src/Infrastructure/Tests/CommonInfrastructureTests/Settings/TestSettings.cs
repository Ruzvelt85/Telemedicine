namespace Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests.Settings
{
    public record TestSettings
    {
        public static TestSettings Default = new();

        public bool IsEnabled { get; init; } = true;

        public string? Property { get; init; }
    }
}
