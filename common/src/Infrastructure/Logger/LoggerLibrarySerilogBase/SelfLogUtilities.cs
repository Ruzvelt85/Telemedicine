using System;
using System.Linq;

namespace Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase
{
    public static class SelfLogUtilities
    {
        /// <summary> We only leave those lines that are relevant to our application.
        /// If you don’t do this, there are a lot of redundant rows in the stack containing stacks inside the libraries.
        /// </summary>
        public static string PrettifyStackTrace(string stackTrace) =>
            string.Join(" ",
                stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(item => item.Contains("Telemedicine")));

        /// <summary> Replace the character of the new line with a space.
        /// </summary>
        public static string ReplaceNewLines(string msg) => msg.Replace(Environment.NewLine, " ");

        /// <summary> Make a message
        /// </summary>
        /// <param name="logMessage">Messages to write to selflog</param>
        /// <param name="stackTrace">Call stack</param>
        public static string BuildSelflog(string logMessage, string stackTrace)
        {
            var msgWithNotNewLine = ReplaceNewLines(logMessage);

            return $@"{{""Type"":""selflog"", ""Message"":""{msgWithNotNewLine}"", ""StackTrace"":""{stackTrace}""}}";
        }
    }
}
