using System;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients
{
    public record GetAppointmentListByPatientIdQuery : IQuery
    {
        public GetAppointmentListByPatientIdQuery(AppointmentListByPatientIdFilterRequestDto filter, PagingRequestDto paging)
        {
            Filter = filter;
            Paging = paging;
        }

        public Guid PatientId { get; init; }

        public AppointmentListByPatientIdFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }
    }
}
