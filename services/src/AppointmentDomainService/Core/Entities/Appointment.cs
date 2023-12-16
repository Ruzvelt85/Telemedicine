using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Infrastructure.EntityBase;
using Telemedicine.Services.AppointmentDomainService.Core.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Core.Events;
using Telemedicine.Services.AppointmentDomainService.Core.Exceptions;
using Telemedicine.Services.AppointmentDomainService.Core.Services;

namespace Telemedicine.Services.AppointmentDomainService.Core.Entities
{
    public class Appointment : IEntity, IAuditable, ILogicallyDeletable, IHasDomainEvents
    {
        private readonly List<DomainEvent> _domainEvents = new();

        // JsonConstructor needs to be able to deserialize seeds when there are more then 1 constructor
        // JsonSerializationException thrown: 'Unable to find a constructor to use for type.
        // A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public Appointment(Guid id, string title, DateTime startDate, TimeSpan duration,
            AppointmentType type, AppointmentStatus status, string description, int rating, Guid? creatorId, DateTime createdOn, DateTime updatedOn, bool isDeleted)
        {
            Id = id;
            Title = title;
            StartDate = startDate;
            Duration = duration;
            Type = type;
            Status = status;
            Description = description;
            Rating = rating;
            CreatorId = creatorId;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            IsDeleted = isDeleted;
            Attendees = new List<AppointmentAttendee>();
        }

        /// <summary>
        /// This ctor is used for create new appointment flow from client side
        /// </summary>
        private Appointment(CreateAppointmentDomainDto dto)
        {
            Title = dto.Title;
            StartDate = dto.StartDate;
            Duration = dto.Duration;
            Description = dto.Description;
            Type = dto.Type;
            Status = AppointmentStatus.Opened;
            CreatorId = dto.CreatorId;
            Attendees = dto.AttendeeIds.Select(attendeeId => new AppointmentAttendee(attendeeId)).ToArray();
        }

        public Guid Id { get; init; }

        public string Title { get; init; }

        public DateTime StartDate { get; init; }

        public TimeSpan Duration { get; init; }

        public AppointmentType Type { get; init; }

        /// <summary>
        /// Status stored in the database
        /// </summary>
        // TODO: Yasnobulkov Property must be hidden JD-1020
        public AppointmentStatus Status { get; set; } // impossible to use private set, because of AutoFixture

        /// <summary>
        /// Current state related to the current point in time
        /// </summary>
        public AppointmentState GetState()
        {
            foreach (var (statePredicate, appointmentState) in AppointmentsByStateSpecification.StatePredicates)
            {
                if (statePredicate.Invoke(this))
                {
                    return appointmentState;
                }
            }

            return AppointmentState.Default;
        }

        public string? CancelReason { get; set; } // impossible to use private set, because of AutoFixture

        /// <summary>
        /// Feedback from a patient. From 1 to 5. Can be null
        /// </summary>
        public int Rating { get; init; }

        public string? Description { get; init; }

        public Guid? CreatorId { get; init; }

        public DateTime CreatedOn { get; init; }

        public DateTime UpdatedOn { get; init; }

        public bool IsDeleted { get; init; }

        public ICollection<AppointmentAttendee> Attendees { get; init; }

        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Returns a new Appointment
        /// </summary>
        /// <exception cref="AppointmentOverlappedException">Thrown when try to create an overlapping appointment</exception>
        public static async Task<Appointment> Create(CreateAppointmentDomainDto dto, IOverlappedAppointmentsService overlappedAppointmentsService, CancellationToken cancellationToken = default)
        {
            Appointment[] overlappedAppointments = await overlappedAppointmentsService.GetOverlappedAppointments(dto.StartDate, dto.Duration, dto.AttendeeIds, cancellationToken);
            if (overlappedAppointments.Any())
            { throw new AppointmentOverlappedException(overlappedAppointments.Select(a => a.Id).ToArray()); }

            return Create(dto);
        }

        /// <summary>
        /// Cancel appointment
        /// </summary>
        /// <exception cref="EntityAlreadyDeletedException">Thrown when appointment was already deleted (property <see cref="IsDeleted"/> = true)</exception>
        /// <returns>Return True if Appointment was cancelled successfully</returns>
        public bool Cancel(string? cancelReason)
        {
            if (IsDeleted)
            { throw new EntityAlreadyDeletedException(typeof(Appointment), Id); }

            if (GetState() == AppointmentState.Cancelled || GetState() == AppointmentState.Missed)
            { return false; }

            Status = GetState() == AppointmentState.Opened ? AppointmentStatus.Cancelled : AppointmentStatus.Missed;
            CancelReason = cancelReason;
            AddDomainEvent(new AppointmentCancelledDomainEvent(this));
            return true;
        }

        /// <summary>
        /// Cancels the current appointment and returns a new one set on the specified time
        /// </summary>
        public async Task<Appointment> Reschedule(RescheduleAppointmentDomainDto dto, IOverlappedAppointmentsService overlappedAppointmentsService, CancellationToken cancellationToken = default)
        {
            await CheckIfCanReschedule(dto.StartDate, dto.Duration, overlappedAppointmentsService, cancellationToken);

            Cancel(dto.Reason);

            var createDto = new CreateAppointmentDomainDto(Title, Description, dto.StartDate, dto.Duration, Type, dto.CreatorId, Attendees.Select(a => a.UserId).ToArray());
            return Create(createDto);
        }

        private void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        private async Task CheckIfCanReschedule(DateTime startDate, TimeSpan duration, IOverlappedAppointmentsService overlappedAppointmentsService, CancellationToken cancellationToken = default)
        {
            if (IsDeleted)
            { throw new EntityAlreadyDeletedException(typeof(Appointment), Id); }

            AppointmentState currentState = GetState();
            if (currentState != AppointmentState.Opened)
            { throw new InvalidAppointmentStateException($"You can't reschedule an appointment in the state '{currentState}'", Id, currentState); }

            Appointment[] overlappedAppointments = await overlappedAppointmentsService.GetOverlappedAppointments(startDate, duration, Attendees.Select(a => a.UserId).ToArray(), cancellationToken);
            Appointment[] overlappedAppointmentsExcludingCurrent = overlappedAppointments.Where(a => a.Id != Id).ToArray();
            if (overlappedAppointmentsExcludingCurrent.Any())
            { throw new AppointmentOverlappedException(Id, overlappedAppointmentsExcludingCurrent.Select(a => a.Id).ToArray()); }
        }

        private static Appointment Create(CreateAppointmentDomainDto dto)
        {
            var newAppointment = new Appointment(dto);
            newAppointment.AddDomainEvent(new AppointmentCreatedDomainEvent(newAppointment));

            return newAppointment;
        }
    }
}
