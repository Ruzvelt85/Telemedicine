using System;

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.ExceptionTypes
{
    /// <summary>
    /// Class for testing purpose that can be serialized
    /// </summary>
    [Serializable]
    internal class SerializableType
    {
        public string? StringProperty { get; set; }
    }
}
