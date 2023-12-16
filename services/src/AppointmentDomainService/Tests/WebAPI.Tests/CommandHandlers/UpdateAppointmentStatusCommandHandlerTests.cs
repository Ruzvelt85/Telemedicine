using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business;
using Telemedicine.Common.Contracts.GlobalContracts.Utilities;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Core.Events;
using Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers.TestSetup;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Commands;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using CommonEnums = Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers
{
    public class UpdateAppointmentStatusCommandHandlerTests : AppointmentCommandHandlerTests
    {
        public UpdateAppointmentStatusCommandHandlerTests() : base($"{nameof(UpdateAppointmentStatusCommandHandlerTests)}-{Guid.NewGuid()}")
        {
        }

        [Fact]
        public async Task CancelAppointment_WhenAppointmentIsOpen_ShouldSetStatusAndCancelReasonAndRaiseDomainEvent()
        {
            // Arrange
            const string cancelReason = "Some reason";

            var appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.Status, AppointmentStatus.Opened)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.StartDate, DateTime.UtcNow.AddDays(1))
                .With(_ => _.Duration, TimeSpan.FromMinutes(45))
                .Create();

            await AddAsync(appointment);
            var domainEventPublisherSpy = new DomainEventPublisherSpy<AppointmentCancelledDomainEvent>();

            // Act
            await using (var context = await GetDbContext())
            {
                var query = new UpdateAppointmentStatusCommand { Id = appointmentId, Status = CommonEnums.AppointmentStatus.Cancelled, Reason = cancelReason };
                var queryHandler = new UpdateAppointmentStatusCommandHandler(new AppointmentReadRepository(context), new AppointmentWriteRepository(context));
                await queryHandler.HandleAsync(query);

                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();
            }

            // Assert
            var savedAppointment = await FindAsync<Appointment>(appointmentId);

            savedAppointment.Should().NotBeNull();
            savedAppointment!.Status.Should().Be(AppointmentStatus.Cancelled);
            savedAppointment.CancelReason.Should().Be(cancelReason);
            savedAppointment.UpdatedOn.Should().BeCloseTo(DateTime.UtcNow, 500.Milliseconds());

            domainEventPublisherSpy.ShouldPublishNumberOfEvents(1)
                .And.ShouldContain(_ => _.Appointment.Id == appointmentId && string.Equals(_.Appointment.CancelReason, cancelReason, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task CancelAppointment_WhenEmptyRepository_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var query = new UpdateAppointmentStatusCommand { Id = appointmentId };

            await using var context = await GetDbContext();
            var queryHandler = new UpdateAppointmentStatusCommandHandler(new AppointmentReadRepository(context), new AppointmentWriteRepository(context));

            // Act
            var ex = await Record.ExceptionAsync(() => queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<EntityNotFoundByIdException>(ex);

            var entityNotFoundEx = (EntityNotFoundByIdException)ex;
            Assert.Equal(appointmentId, entityNotFoundEx.Id);
            Assert.Equal(typeof(Appointment).FullName, entityNotFoundEx.Type);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), entityNotFoundEx.Code);
        }

        [Fact]
        public async Task CancelAppointment_WhenDeletedAppointment_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.IsDeleted, true)
                .Create();

            await AddAsync(appointment);
            await using var context = await GetDbContext();
            var queryHandler = new UpdateAppointmentStatusCommandHandler(new AppointmentReadRepository(context), new AppointmentWriteRepository(context));
            var query = new UpdateAppointmentStatusCommand() { Id = appointmentId, Reason = "reason", Status = CommonEnums.AppointmentStatus.Cancelled };

            // Act
            var ex = await Record.ExceptionAsync(() => queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(ex);
            Assert.IsType<EntityNotFoundByIdException>(ex);
            var entityAlreadyDeletedEx = (EntityNotFoundByIdException)ex;
            Assert.Equal(appointmentId, entityAlreadyDeletedEx.Id);
            Assert.Equal(typeof(Appointment).FullName, entityAlreadyDeletedEx.Type);
            Assert.Equal(BusinessException.ErrorType.EntityNotFound.ToErrorCodeString(), entityAlreadyDeletedEx.Code);
        }

        [Theory]
        [InlineData(3600, 3600, AppointmentStatus.Opened, AppointmentStatus.Cancelled, "Cancel appointment before start should have cancelled status")]
        [InlineData(-600, 3600, AppointmentStatus.Opened, AppointmentStatus.Missed, "Cancel already ongoing appointment should have missed status")]
        [InlineData(-3600, 600, AppointmentStatus.Opened, AppointmentStatus.Missed, "Cancel already done appointment should have missed status")]
        [InlineData(-3600, 600, AppointmentStatus.Missed, AppointmentStatus.Missed, "Cancel already missed appointment should have missed status")]
        [InlineData(0, 3600, AppointmentStatus.Cancelled, AppointmentStatus.Cancelled, "Cancel already cancelled should have cancelled status")]
        public async Task Cancel_InDifferentTime_ShouldHaveCorrectStatus(int timeBeforeStartInSec, int durationInSec,
            AppointmentStatus currentStatus, AppointmentStatus expectedStatus, string userMessage)
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var newAppointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, DateTime.UtcNow.AddSeconds(timeBeforeStartInSec))
                .With(_ => _.Duration, TimeSpan.FromSeconds(durationInSec))
                .With(_ => _.Status, currentStatus)
                .With(_ => _.IsDeleted, false)
                .Create();

            await AddAsync(newAppointment);

            // Act
            await using (var context = await GetDbContext())
            {
                var query = new UpdateAppointmentStatusCommand { Id = appointmentId, Status = CommonEnums.AppointmentStatus.Cancelled };
                var queryHandler = new UpdateAppointmentStatusCommandHandler(new AppointmentReadRepository(context), new AppointmentWriteRepository(context));
                await queryHandler.HandleAsync(query);
                var domainEventPublisherSpy = new DomainEventPublisherSpy<AppointmentCancelledDomainEvent>();
                var unitOfWork = new UnitOfWork<AppointmentDomainServiceDbContext>(context, domainEventPublisherSpy);
                await unitOfWork.SaveAsync();
            }

            // Assert
            var savedAppointment = await FindAsync<Appointment>(appointmentId);

            savedAppointment.Should().NotBeNull();
            savedAppointment!.Status.Should().Be(expectedStatus, userMessage);
        }
    }
}
