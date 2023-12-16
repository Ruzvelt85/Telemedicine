using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public class MeasurementListRequestDtoValidator : AbstractValidator<GetMeasurementListRequestDto>
    {
        public MeasurementListRequestDtoValidator()
        {
            RuleFor(_ => _.Paging).SetValidator(new PagingRequestDtoValidator());
            RuleFor(_ => _.Filter).SetValidator(new MeasurementListFilterRequestDtoValidator());
        }
    }
}
