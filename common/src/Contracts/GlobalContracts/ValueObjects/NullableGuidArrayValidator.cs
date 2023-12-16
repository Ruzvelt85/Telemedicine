using System;
using System.Linq;
using FluentValidation;

namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    /// <summary>
    /// Defines validator for nullable array of Guid
    /// </summary>
    public class NullableGuidArrayValidator : AbstractValidator<Guid[]?>
    {
        public NullableGuidArrayValidator(int maxArrayLength)
        {
            RuleFor(_ => _).Must(array => array is null || !array.Any()
                                                        || (array.Length <= maxArrayLength && array.All(item => item != Guid.Empty)))
                .WithMessage("The array of GUID must be of size between 0 and 100 non-empty elements.");
        }
    }
}
