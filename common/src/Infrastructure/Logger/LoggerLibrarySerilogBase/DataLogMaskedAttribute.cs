using Destructurama.Attributed;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase
{
    public class DataLogMaskedAttribute : LogMaskedAttribute
    {
        public DataLogMaskedAttribute()
        {
            ShowFirst = 1;
        }
    }
}
