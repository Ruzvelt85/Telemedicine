using System;
using Newtonsoft.Json;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.JsonConverters
{
    internal static class TimeSpanToMillisecondsJsonConverterHelper
    {
        internal static void WriteJson(JsonWriter writer, TimeSpan value)
        {
            writer.WriteValue((long)value.TotalMilliseconds);
        }

        internal static TimeSpan ReadJson(JsonReader reader)
        {
            return reader.Value is not null ? TimeSpan.FromMilliseconds((long)reader.Value) : TimeSpan.Zero;
        }
    }
}
