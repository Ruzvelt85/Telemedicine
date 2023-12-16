using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId
{
    public record AppointmentListByPatientIdRequestDto
    {
        public AppointmentListByPatientIdRequestDto()
        {
            Filter = new AppointmentListByPatientIdFilterRequestDto();
            Paging = new PagingRequestDto();
        }

        public AppointmentListByPatientIdFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }
    }
}
