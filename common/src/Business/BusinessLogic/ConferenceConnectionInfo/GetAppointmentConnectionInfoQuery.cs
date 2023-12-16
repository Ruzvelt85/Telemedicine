using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo
{
    public record GetAppointmentConnectionInfoQuery(Guid Id) : IQuery;
}
