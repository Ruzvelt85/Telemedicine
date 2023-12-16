using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.HealthMeasurementDomainService.API.Common
{
    public class GetMeasurementListRequestDtoValidator : AbstractValidator<GetMeasurementListRequestDto>
    {
        public GetMeasurementListRequestDtoValidator()
        {
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
            RuleFor(_ => _.Filter).SetValidator(new MeasurementListFilterRequestDtoValidator());
        }
    }
}
