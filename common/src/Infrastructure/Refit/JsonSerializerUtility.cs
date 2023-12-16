using Telemedicine.Common.Infrastructure.CommonInfrastructure.JsonConverters;
using Newtonsoft.Json;

namespace Telemedicine.Common.Infrastructure.Refit
{
    public static class JsonSerializerUtility
    {
        /// <summary>
        /// Create and configure <see cref="JsonSerializerSettings"/>
        /// </summary>
        public static JsonSerializerSettings CreateAndConfigureJsonSerializerSettings()
        {
            return new JsonSerializerSettings().ConfigureJsonSerializerSettings();
        }

        /// <summary>
        /// Configure Json Serializer settings like converting all Enum values into string and so on
        /// </summary>
        public static JsonSerializerSettings ConfigureJsonSerializerSettings(this JsonSerializerSettings serializerSettings)
        {
            serializerSettings.Converters.Add(new TimeSpanToTotalMillisecondsJsonConverter());
            serializerSettings.Converters.Add(new TimeSpanNullableToTotalMillisecondsJsonConverter());
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return serializerSettings;
        }
    }
}
