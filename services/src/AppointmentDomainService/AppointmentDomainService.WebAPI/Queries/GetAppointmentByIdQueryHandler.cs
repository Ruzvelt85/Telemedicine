using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries
{
    public class GetAppointmentByIdQueryHandler : IQueryHandler<GetAppointmentByIdQuery, AppointmentByIdResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<Appointment> _appointmentReadRepository;

        public GetAppointmentByIdQueryHandler(IMapper mapper, IReadRepository<Appointment> appointmentReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appointmentReadRepository = appointmentReadRepository ?? throw new ArgumentNullException(nameof(appointmentReadRepository));
        }

        /// <inheritdoc />
        public async Task<AppointmentByIdResponseDto> HandleAsync(GetAppointmentByIdQuery query, CancellationToken cancellationToken = default)
        {
            var appointment = await _appointmentReadRepository
                .Find(a => a.Id == query.Id)
                .Include(a => a.Attendees)
                .FirstOrDefaultAsync(cancellationToken);

            if (appointment == null)
            { throw new EntityNotFoundByIdException(typeof(Appointment), query.Id); }

            return _mapper.Map<AppointmentByIdResponseDto>(appointment);
        }
    }
}
