using System;
using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;

namespace Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId
{
    public record AppointmentListByPatientIdResponseDto
    {
        public IReadOnlyCollection<AppointmentResponseDto> Items { get; init; } = Array.Empty<AppointmentResponseDto>();

        public PagingResponseDto Paging { get; init; } = new(0);
    }
}
