using System;

namespace Telemedicine.Common.Contracts.GlobalContracts.Utilities
{
    public static class ExceptionUtilities
    {
        public static string ToErrorCodeString(this Enum code)
        {
            return code.ToString("G");
        }
    }
}
