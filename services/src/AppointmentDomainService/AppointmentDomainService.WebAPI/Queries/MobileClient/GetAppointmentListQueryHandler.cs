using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.MobileClient
{
    public class GetAppointmentListQueryHandler : IQueryHandler<GetAppointmentListQuery, AppointmentListResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<Appointment> _appointmentReadRepository;

        public GetAppointmentListQueryHandler(IMapper mapper, IReadRepository<Appointment> appointmentReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentReadRepository = appointmentReadRepository ?? throw new ArgumentNullException(nameof(appointmentReadRepository));
        }

        /// <inheritdoc />
        public async Task<AppointmentListResponseDto> HandleAsync(GetAppointmentListQuery query, CancellationToken cancellationToken = default)
        {
            var appointmentsByPatientSpec = new AppointmentsByAttendeeSpecification(query.AttendeeId);
            var activeAppointmentsSpec = new ActiveAppointmentsSpecification();

            var appointments = await _appointmentReadRepository
                .Find(appointmentsByPatientSpec & activeAppointmentsSpec)
                .Include(_ => _.Attendees)
                .OrderByDescending(el => el.StartDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<AppointmentListResponseDto>(appointments);
        }
    }
}
