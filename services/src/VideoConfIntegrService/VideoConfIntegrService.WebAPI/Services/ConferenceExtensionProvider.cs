using System;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    public static class ConferenceExtensionProvider
    {
        /// <summary>
        /// Generates unique extension for creating a new room
        /// </summary>
        /// <param name="extensionPrefix">Fixed prefix for extension</param>
        public static string GenerateExtension(string? extensionPrefix)
        {
            if (string.IsNullOrWhiteSpace(extensionPrefix) || !long.TryParse(extensionPrefix, out long result) || result < 1)
            { throw new ArgumentException(nameof(extensionPrefix)); }

            return $"{extensionPrefix}{DateTime.UtcNow.Ticks}";
        }
    }
}
