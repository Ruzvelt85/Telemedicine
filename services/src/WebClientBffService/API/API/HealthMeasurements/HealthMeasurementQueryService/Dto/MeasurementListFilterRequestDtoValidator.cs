using System;
using FluentValidation;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto
{
    public class MeasurementListFilterRequestDtoValidator : AbstractValidator<MeasurementListFilterRequestDto>
    {
        public MeasurementListFilterRequestDtoValidator()
        {
            RuleFor(_ => _.PatientId).NotEmpty();
            RuleFor(_ => _.DateRange).SetValidator(new RangeValidator<DateTime>());
            RuleFor(_ => _.MeasurementType).IsInEnum();
        }
    }
}
