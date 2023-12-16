namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.ExceptionTypes
{
    /// <summary>
    /// Class for testing purpose that can not be serialized
    /// </summary>
    internal class NonSerializableType
    {
        public string? StringProperty { get; set; }
    }
}
