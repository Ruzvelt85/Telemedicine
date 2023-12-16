using FluentValidation;

namespace Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto
{
    public class CreateConferenceRequestDtoValidator : AbstractValidator<CreateConferenceRequestDto>
    {
        public CreateConferenceRequestDtoValidator()
        {
            RuleFor(_ => _.AppointmentId).NotEmpty();
            RuleFor(_ => _.AppointmentTitle).NotEmpty().MaximumLength(100);
            RuleFor(_ => _.AppointmentStartDate).NotEmpty();
            RuleFor(_ => _.AppointmentDuration).NotEmpty();
        }
    }
}
