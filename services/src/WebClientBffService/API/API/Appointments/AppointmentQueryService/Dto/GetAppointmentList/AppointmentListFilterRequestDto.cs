using System;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;

namespace Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList
{
    public record AppointmentListFilterRequestDto
    {
        public Range<DateTime?> DateRange { get; init; } = new();

        public AppointmentStatus AppointmentStatus { get; init; } = AppointmentStatus.All;
    }
}
