using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Queries
{
    public record GetLastChangedDataQuery : IQuery
    {
        public DateTime? AppointmentsLastUpdate { get; init; }

        public DateTime? MoodLastUpdate { get; init; }
    }
}
