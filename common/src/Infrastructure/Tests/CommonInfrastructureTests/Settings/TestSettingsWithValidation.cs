namespace Telemedicine.Common.Infrastructure.Tests.CommonInfrastructureTests.Settings
{
    public record TestSettingsWithValidation
    {
        public static TestSettingsWithValidation Default = new();

        public bool IsEnabled { get; init; } = true;

        public string? Property { get; init; }
    }
}
