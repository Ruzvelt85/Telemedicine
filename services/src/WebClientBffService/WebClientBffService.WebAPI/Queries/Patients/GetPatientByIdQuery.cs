using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients
{
    public record GetPatientByIdQuery : IQuery
    {
        public GetPatientByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; init; }
    }
}
