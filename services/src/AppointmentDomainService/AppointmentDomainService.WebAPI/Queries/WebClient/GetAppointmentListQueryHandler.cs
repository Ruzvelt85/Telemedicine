using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.WebClient
{
    public class GetAppointmentListQueryHandler : IQueryHandler<GetAppointmentListQuery, AppointmentListResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IReadRepository<Appointment> _appointmentReadRepository;

        public GetAppointmentListQueryHandler(IMapper mapper, IReadRepository<Appointment> appointmentReadRepository)
        {
            _mapper = mapper;
            _appointmentReadRepository = appointmentReadRepository;
        }

        /// <inheritdoc />
        public async Task<AppointmentListResponseDto> HandleAsync(GetAppointmentListQuery query, CancellationToken cancellationToken = default)
        {
            Specification<Appointment> appointmentsWithSpecifiedAttendee = new AppointmentsByAttendeeSpecification(query.Filter.AttendeeId);
            Specification<Appointment> appointmentsInSpecifiedDateRange = new AppointmentsInDateRangeSpecification(query.Filter.DateRange);
            Specification<Appointment> appointmentsInSpecifiedState = GetByStateOrTrueSpecification(query.Filter.AppointmentStates);

            var specification = appointmentsWithSpecifiedAttendee & appointmentsInSpecifiedDateRange & appointmentsInSpecifiedState;
            var appointments = await _appointmentReadRepository
                .Find(specification)
                .Include(x => x.Attendees)
                .OrderByDescending(x => x.StartDate)
                .Paginate(query.Paging.Skip, query.Paging.Take)
                .ToListAsync(cancellationToken);

            var totalCount = await _appointmentReadRepository.CountAsync(specification, cancellationToken);

            return _mapper.Map<AppointmentListResponseDto>(appointments) with { Paging = new PagingResponseDto(totalCount) };
        }

        // TODO Yasnobulkov JD-993 Refactoring and add tests
        private Specification<Appointment> GetByStateOrTrueSpecification(IReadOnlyCollection<API.Common.Common.AppointmentState>? appointmentStates)
        {
            if (appointmentStates == null || appointmentStates.Count == 0)
            { return new TrueSpecification<Appointment>(); }

            // TODO: Savich - use flags instead of states collection
            if (appointmentStates.Contains(API.Common.Common.AppointmentState.All))
            { return new TrueSpecification<Appointment>(); }

            var specifications = appointmentStates
                .Select(state => new AppointmentsByStateSpecification(_mapper.Map<AppointmentState>(state)))
                .ToList<Specification<Appointment>>();

            var result = specifications.Aggregate((aggregate, spec) => aggregate |= spec);
            return result;
        }
    }
}
