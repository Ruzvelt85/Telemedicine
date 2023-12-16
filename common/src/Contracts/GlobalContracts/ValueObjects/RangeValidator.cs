using System;
using FluentValidation;

namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    public class RangeValidator<T> : AbstractValidator<Range<T?>> where T : struct, IComparable
    {
        public RangeValidator()
        {
#pragma warning disable CS8629 // Nullable value type may be null.
            RuleFor(_ => _.To)
                .Must((range, el) => el.Value.CompareTo(range.From) > 0) // 'To' must be more then 'From'
                .When(range => range.To?.Equals(default(T)) == false) // Only when 'To' have a value (not null and not default)
                .When(range => range.From?.Equals(default(T)) == false); // And only when 'From' have a value (not null and not default)
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
