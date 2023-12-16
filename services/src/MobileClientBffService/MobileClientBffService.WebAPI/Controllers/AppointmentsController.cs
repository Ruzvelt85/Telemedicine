using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.MobileClientBffService.API.AppointmentQueryService;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : BffServiceBaseController, IAppointmentQueryService
    {
        private readonly IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto> _getAppointmentConnectionInfoQueryHandler;

        public AppointmentsController(IQueryHandler<GetAppointmentConnectionInfoQuery, AppointmentConnectionInfoResponseDto> getAppointmentConnectionInfoQueryHandler)
        {
            _getAppointmentConnectionInfoQueryHandler = getAppointmentConnectionInfoQueryHandler ?? throw new ArgumentNullException(nameof(getAppointmentConnectionInfoQueryHandler));
        }

        [HttpGet("connectioninfo/{appointmentId}")]
        public async Task<AppointmentConnectionInfoResponseDto> GetAppointmentConnectionInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default)
        {
            var result = await _getAppointmentConnectionInfoQueryHandler.HandleAsync(new GetAppointmentConnectionInfoQuery(appointmentId), cancellationToken);
            return result;
        }
    }
}
