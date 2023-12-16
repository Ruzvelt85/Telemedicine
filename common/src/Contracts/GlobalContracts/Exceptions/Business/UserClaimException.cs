using System;
using System.Collections;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business
{
    [Serializable]
    public class UserClaimException : BusinessException
    {
        [PublicAPI]
        public new enum ErrorType
        {
            ClaimDoesnotExist = 0,
            ClaimValueIsInvalid = 1
        }

        public UserClaimException(string message, Enum code, string claimType, string? claimValue = null, Exception? innerException = null) : base(message, code, innerException)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public UserClaimException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        [Obsolete(DoNotUseCtorMessage, true)]
        public UserClaimException(string message, IDictionary data, Exception? innerException = null) : base(message, data, innerException)
        {
        }

        public string ClaimType
        {
            get => GetProperty<string>()!;
            set => SetProperty(value);
        }

        public string? ClaimValue
        {
            get => GetProperty<string>()!;
            set => SetProperty(value);
        }
    }
}
