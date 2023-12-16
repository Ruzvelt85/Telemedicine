using System.Diagnostics;

namespace Telemedicine.Common.Infrastructure.Testing
{
    /// <summary>
    /// Base class for unit tests
    /// </summary>
    public class UnitTestsBase
    {
        public UnitTestsBase()
        {
            RemoveDefaultTraceListener();
        }

        /// <summary>
        /// This method is needed to fix issue with Debug.Assert in tested code
        /// </summary>
        private static void RemoveDefaultTraceListener()
        {
            Trace.Listeners.Remove("Default");
        }
    }
}
