using System;

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.ExceptionTypes
{
    /// <summary>
    /// Class for testing purpose that can be serialized, but contains not serializable property
    /// </summary>
    [Serializable]
    internal class SerializableTypeWithNonSerializableProperty
    {
        public Type? TypeProperty { get; set; }
    }
}
