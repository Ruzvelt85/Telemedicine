using System;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    public static class ConferencePinCodeGenerator
    {
        /// <summary>
        /// Generates random PIN-code for conference
        /// </summary>
        /// <returns></returns>
        public static string Generate(int digitQuantity)
        {
            if (digitQuantity < 6 || digitQuantity > 12)
            { digitQuantity = 10; }

            // We should support possibility to specify 12-digits format of PIN-code
            // But size of INT32 is less than 12 digits, and current version of .NET has no method of Random.Next() for type long
            // Therefore we use two-step algorithm of creating of PIN-code
            var firstPart = new Random().Next(0, 999999).ToString("D6");

            if (digitQuantity == 6)
            { return firstPart; }

            var secondPart = new Random().Next(0, int.Parse(new string('9', (digitQuantity - 6)))).ToString($"D{digitQuantity - 6}");
            return $"{firstPart}{secondPart}";
        }
    }
}
