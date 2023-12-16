namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup
{
    public record ServiceStartupSettings
    {
        public static ServiceStartupSettings Default = new();

        public bool IsSwaggerEnabled { get; init; } = true;

        public string? SwaggerPathPrefix { get; init; }
    }
}
