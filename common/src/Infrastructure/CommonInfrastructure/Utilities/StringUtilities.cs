using System.Text.RegularExpressions;

namespace Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities
{
    public static class StringUtilities
    {
        public static string ToLowerAndTrim(this string value)
        {
            return value.ToLower().Trim();
        }

        public static string RemoveDuplicateSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
    }
}
