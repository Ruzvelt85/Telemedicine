using System;
using Newtonsoft.Json;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.JsonConverters
{
    public class TimeSpanToTotalMillisecondsJsonConverter : JsonConverter<TimeSpan>
    {
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            TimeSpanToMillisecondsJsonConverterHelper.WriteJson(writer, value);
        }

        /// <inheritdoc />
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return TimeSpanToMillisecondsJsonConverterHelper.ReadJson(reader);
        }
    }
}
