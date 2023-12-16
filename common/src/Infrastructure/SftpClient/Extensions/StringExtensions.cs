using System;
using System.IO;

namespace Telemedicine.Common.Infrastructure.SftpClient.Extensions
{
    internal static class StringExtensions
    {
        public static Stream GetStream(this string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(inputString));
            }

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(inputString);
            streamWriter.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
