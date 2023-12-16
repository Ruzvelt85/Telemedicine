using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.MobileClient
{
    public class GetChangedAppointmentListQueryHandler : IQueryHandler<GetChangedAppointmentListQuery, AppointmentListResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IAppointmentReadRepository _appointmentReadRepository;

        public GetChangedAppointmentListQueryHandler(IMapper mapper, IAppointmentReadRepository appointmentReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentReadRepository = appointmentReadRepository ?? throw new ArgumentNullException(nameof(appointmentReadRepository));
        }

        /// <inheritdoc />
        public async Task<AppointmentListResponseDto> HandleAsync(GetChangedAppointmentListQuery query, CancellationToken cancellationToken = default)
        {
            var appointmentsByPatientSpec = new AppointmentsByAttendeeSpecification(query.AttendeeId);
            var fromLastUpdateSpec = new AppointmentsFromLastUpdateSpecification(query.LastUpdate);

            var appointments = await _appointmentReadRepository
                .FindWithDeleted(fromLastUpdateSpec & appointmentsByPatientSpec)
                .Include(a => a.Attendees)
                .OrderByDescending(el => el.StartDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<AppointmentListResponseDto>(appointments);
        }
    }
}
