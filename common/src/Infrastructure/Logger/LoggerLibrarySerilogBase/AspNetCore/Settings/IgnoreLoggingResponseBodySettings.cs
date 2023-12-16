namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.AspNetCore.Settings
{
    public record IgnoreLoggingResponseBodySettings
    {
        public string? HttpVerb { get; init; }

        public string? Path { get; init; }
    }
}
