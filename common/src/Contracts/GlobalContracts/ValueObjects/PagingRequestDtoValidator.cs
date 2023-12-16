using FluentValidation;

namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    public class PagingRequestDtoValidator : AbstractValidator<PagingRequestDto>
    {
        public PagingRequestDtoValidator()
        {
            RuleFor(_ => _.Skip).NotNull().GreaterThanOrEqualTo(0);
            RuleFor(_ => _.Take).NotNull().GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        }
    }
}
