using System;
using FluentValidation;

namespace Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto
{
    public class LastChangedDataRequestDtoValidator : AbstractValidator<LastChangedDataRequestDto>
    {
        public LastChangedDataRequestDtoValidator()
        {
            RuleFor(_ => _.AppointmentsLastUpdate).NotEqual(default(DateTime));
            RuleFor(_ => _.MoodLastUpdate).NotEqual(default(DateTime));
        }
    }
}
