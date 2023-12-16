using System;
using Newtonsoft.Json;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.JsonConverters
{
    public class TimeSpanNullableToTotalMillisecondsJsonConverter : JsonConverter<TimeSpan?>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, TimeSpan? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteValue(value);
                return;
            }

            TimeSpanToMillisecondsJsonConverterHelper.WriteJson(writer, value.Value);
        }

        /// <inheritdoc />
        public override TimeSpan? ReadJson(JsonReader reader, Type objectType, TimeSpan? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is null)
            { return null; }

            return TimeSpanToMillisecondsJsonConverterHelper.ReadJson(reader);
        }
    }
}
