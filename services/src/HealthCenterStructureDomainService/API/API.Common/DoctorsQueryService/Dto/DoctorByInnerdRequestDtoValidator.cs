using FluentValidation;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto
{
    public class DoctorByInnerIdRequestDtoValidator : AbstractValidator<DoctorByInnerIdRequestDto>
    {
        public DoctorByInnerIdRequestDtoValidator()
        {
            RuleFor(_ => _.InnerId).NotEmpty().MaximumLength(FieldLengthConstants.InnerIdLength);
        }
    }
}
