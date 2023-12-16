using System;
using System.Collections;
using System.Runtime.Serialization;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
// ReSharper disable UnusedMember.Global

namespace Telemedicine.Common.Contracts.Tests.GlobalContracts.ExceptionTypes
{
    /// <summary>
    /// Represent exception for test purpose only to check property serialization
    /// </summary>
    internal class TestException : ServiceLayerException
    {
        public enum ErrorCode
        {
            TestError
        }

        /// <inheritdoc />
        public TestException(string message, Enum code, Exception? innerException = null)
            : base(message, code, innerException)
        {
        }

        /// <inheritdoc />
        public TestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, false)]
        public TestException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public string? StringProperty
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public byte ByteProperty
        {
            get => GetProperty<byte>();
            set => SetProperty(value);
        }

        public long LongProperty
        {
            get => GetProperty<long>();
            set => SetProperty(value);
        }

        public bool BoolProperty
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        public DateTime DateTimeProperty
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        public TimeSpan TimeSpanProperty
        {
            get => GetProperty<TimeSpan>();
            set => SetProperty(value);
        }

        public long TimeStampProperty
        {
            get => GetProperty<long>();
            set => SetProperty(value);
        }

        public Guid GuidProperty
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }

        public byte[]? ByteArrayProperty
        {
            get => GetProperty<byte[]>();
            set => SetProperty(value);
        }

        public int[]? IntArrayProperty
        {
            get => GetProperty<int[]>();
            set => SetProperty(value);
        }

        public Type? TypeProperty
        {
            get => GetProperty<Type>();
            set => SetProperty(value);
        }

        public object? NullProperty
        {
            get => GetProperty<object>();
            set => SetProperty(value);
        }

        public SerializableType? SerializableTypeProperty
        {
            get => GetProperty<SerializableType>();
            set => SetProperty(value);
        }

        public NonSerializableType? NonSerializableTypeProperty
        {
            get => GetProperty<NonSerializableType>();
            set => SetProperty(value);
        }
        public SerializableTypeWithNonSerializableProperty? SerializableTypeWithNonSerializableProperty
        {
            get => GetProperty<SerializableTypeWithNonSerializableProperty>();
            set => SetProperty(value);
        }
    }
}
