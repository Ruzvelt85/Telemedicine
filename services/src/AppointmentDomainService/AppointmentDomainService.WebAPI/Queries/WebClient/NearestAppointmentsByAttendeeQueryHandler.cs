using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.WebClient
{
    public class NearestAppointmentsByAttendeeQueryHandler : IQueryHandler<NearestAppointmentsByAttendeeQuery, NearestAppointmentsResponseDto>
    {
        private readonly IReadRepository<Appointment> _appointmentReadRepository;
        private readonly IMapper _mapper;

        public NearestAppointmentsByAttendeeQueryHandler(IReadRepository<Appointment> appointmentReadRepository, IMapper mapper)
        {
            _appointmentReadRepository = appointmentReadRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<NearestAppointmentsResponseDto> HandleAsync(NearestAppointmentsByAttendeeQuery query, CancellationToken cancellationToken = default)
        {
            var currentDateTime = DateTime.UtcNow;
            var appointmentsWithSpecifiedPatient = new AppointmentsByAttendeeSpecification(query.AttendeeId);
            var previousAppointments = new PreviousAppointmentsSpecification(currentDateTime);
            var nextAppointments = new NextAppointmentsSpecification(currentDateTime);

            var previousAppointment = await _appointmentReadRepository.Find(appointmentsWithSpecifiedPatient & previousAppointments)
                .OrderByDescending(a => a.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            var nextAppointment = await _appointmentReadRepository.Find(appointmentsWithSpecifiedPatient & nextAppointments)
                .OrderBy(a => a.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            return new NearestAppointmentsResponseDto
            {
                AttendeeId = query.AttendeeId,
                PreviousAppointmentInfo = previousAppointment != null ? _mapper.Map<NearestAppointmentInfoResponseDto>(previousAppointment) : null,
                NextAppointmentInfo = nextAppointment != null ? _mapper.Map<NearestAppointmentInfoResponseDto>(nextAppointment) : null,
                NextAppointmentType = nextAppointment != null ? _mapper.Map<AppointmentType>(nextAppointment.Type) : null
            };
        }
    }
}
