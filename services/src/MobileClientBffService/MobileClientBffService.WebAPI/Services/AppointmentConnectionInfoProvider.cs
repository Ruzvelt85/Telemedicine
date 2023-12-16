using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Services
{
    public class AppointmentConnectionInfoProvider : IAppointmentConnectionInfoProvider
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentQueryService _appointmentQueryService;
        private readonly IVideoConferenceQueryService _videoConferenceQueryService;

        public AppointmentConnectionInfoProvider(IMapper mapper, IAppointmentQueryService appointmentQueryService, IVideoConferenceQueryService videoConferenceQueryService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentQueryService = appointmentQueryService ?? throw new ArgumentNullException(nameof(appointmentQueryService));
            _videoConferenceQueryService = videoConferenceQueryService ?? throw new ArgumentNullException(nameof(videoConferenceQueryService));
        }

        public async Task<AppointmentInfoDto> GetAppointmentInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default)
        {
            AppointmentDomainService.API.Common.AppointmentQueryService.Dto.AppointmentByIdResponseDto appointmentResponse = await _appointmentQueryService.GetAppointmentByIdAsync(appointmentId, cancellationToken);
            return _mapper.Map<AppointmentInfoDto>(appointmentResponse);
        }

        public async Task<AppointmentConnectionInfoResponseDto> GetConnectionInfoAsync(Guid appointmentId, CancellationToken cancellationToken = default)
        {
            ConnectionInfoResponseDto connectionInfoResponse = await _videoConferenceQueryService.GetConnectionInfoAsync(appointmentId, cancellationToken);
            return _mapper.Map<AppointmentConnectionInfoResponseDto>(connectionInfoResponse);
        }
    }
}
