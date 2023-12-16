using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Exceptions;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Specifications;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Queries
{
    public class GetConnectionInfoQueryHandler : IQueryHandler<GetConnectionInfoQuery, ConnectionInfoResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<Conference> _conferenceReadRepository;

        public GetConnectionInfoQueryHandler(IMapper mapper, IReadRepository<Conference> conferenceReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _conferenceReadRepository = conferenceReadRepository ?? throw new ArgumentNullException(nameof(conferenceReadRepository));
        }

        /// <inheritdoc />
        public async Task<ConnectionInfoResponseDto> HandleAsync(GetConnectionInfoQuery query, CancellationToken cancellationToken = default)
        {
            Specification<Conference> conferenceWithSpecifiedAppointmentId = new ConferenceWithSpecifiedAppointmentId(query.AppointmentId);

            Conference? existingConference = await _conferenceReadRepository
                .FirstOrDefaultAsync(conferenceWithSpecifiedAppointmentId, cancellationToken);

            if (existingConference is null)
            { throw new VideoConferenceNotFoundByAppointmentIdException(typeof(Conference), query.AppointmentId); }

            return MapToConnectionInfoResponseDto(existingConference);
        }

        private ConnectionInfoResponseDto MapToConnectionInfoResponseDto(Conference conference)
        {
            var result = _mapper.Map<ConnectionInfoResponseDto>(conference);

            var uri = new Uri(conference.RoomUrl);
            result.Host = $"{uri.Scheme}://{uri.Host}";
            result.RoomKey = uri.Segments.Last();

            return result;
        }
    }
}
