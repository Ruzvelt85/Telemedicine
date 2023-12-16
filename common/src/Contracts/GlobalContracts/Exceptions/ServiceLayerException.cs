using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Serilog;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions
{
    /// <summary>
    /// Represent base service layer exception
    /// </summary>
    [Serializable]
    public abstract class ServiceLayerException : Exception
    {
        public const string NullValue = "<NULL>";

        private readonly ILogger _logger = Log.ForContext<ServiceLayerException>();

        private enum ErrorType
        {
            EmptyCode
        }

        protected const string DoNotUseCtorMessage = "Don't use this constructor. The constructor is used to automatically restore the exception.";

        private ServiceLayerException(string message, string code, Exception? innerException = null) : base(message, innerException)
        {
            SetCode(code);
        }

        protected ServiceLayerException(string message, Enum code, Exception? innerException = null)
            : this(message, code.ToErrorCodeString(), innerException)
        {
        }

        /// <inheritdoc />
        protected ServiceLayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// This constructor is used to automatically restore the exception, so it is marked as obsolete
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <param name="innerException"></param>
        [Obsolete(DoNotUseCtorMessage, true)]
        protected ServiceLayerException(string message, IDictionary data, Exception? innerException = null) : this(message, ErrorType.EmptyCode, innerException)
        {
            AddData(data);
        }

        /// <summary>
        /// Code of error (exception)
        /// </summary>
        public virtual string Code
        {
            get
            {
                var result = GetProperty<string>();
                return !string.IsNullOrWhiteSpace(result) ? result : ErrorType.EmptyCode.ToErrorCodeString();
            }
            set => SetProperty(value);
        }

        /// <summary>
        /// Add property value to <see cref="Exception.Data"/> dictionary with key as propertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(T? value, [CallerMemberName] string? propertyName = null)
        {
            SetDataItem(propertyName, value);
        }

        /// <summary>
        /// Get property value from <see cref="Exception.Data"/> dictionary by propertyName (key)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T? GetProperty<T>([CallerMemberName] string? propertyName = null)
        {
            if (propertyName == null)
            {
                return default;
            }

            if (!Data.Contains(propertyName))
            {
                return default;
            }

            var retrievedValue = Data[propertyName];
            switch (retrievedValue)
            {
                case T value:
                    return value;
                default: return default;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            try
            {
                var sb = new StringBuilder(base.ToString());
                sb.Append("Properties:[");
                //Value of Data cannot be null
                foreach (DictionaryEntry kvp in Data)
                {
                    sb.AppendLine();
                    sb.AppendFormat(CultureInfo.InvariantCulture, "Name:{0} Value:{1}", kvp.Key, kvp.Value);
                }
                sb.AppendLine();
                sb.Append(']');
                return sb.ToString();
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Exception occurred during ToString(): {Exception}", exception);

                try
                {
                    return base.ToString();
                }
                catch (Exception internalException)
                {
                    _logger.Error(internalException, "Exception occurred during  base.ToString(): {InternalException}", internalException);
                    return NullValue;
                }
            }
        }

        /// <summary>
        /// Fill in <see cref="Exception.Data"/> dictionary according data from dictionary from method parameter
        /// </summary>
        /// <param name="data"></param>
        private void AddData(IDictionary data)
        {
            foreach (DictionaryEntry entry in data)
            {
                SetDataItem(entry.Key, entry.Value);
            }
        }

        private void SetDataItem<TKey, T>(TKey key, T? value)
        {
            if (key == null)
            {
                const string message = "'Key' parameter is NULL.";
                _logger.Warning(message);
                Debug.Assert(false, message);
                return;
            }

            if (value != null && !value.GetType().IsSerializable)
            {
                const string message = "Value is not serializable.";
                _logger.Warning(message);
                Debug.Assert(false, message);
                return;
            }

            Data[key] = value;
        }

        private void SetCode(string code)
        {
            Code = code;
        }
    }
}
