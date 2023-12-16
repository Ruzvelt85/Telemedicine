using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentCommandService.Dto;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class AppointmentsCommandIntegrationTests : IHttpServiceTests<IAppointmentCommandService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        private readonly IAppointmentCommandService _service;
        private readonly AppointmentDomainServiceDbContext _dbContext;
        private readonly Guid _currentUserId;

        public AppointmentsCommandIntegrationTests(HttpServiceFixture<IAppointmentCommandService> httpServiceFixture, EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            _service = HttpServiceFixture.GetRestService();
            _dbContext = EfCoreContextFixture.DbContext;
            _currentUserId = Guid.Parse("AAB4ED21-0933-4D1F-ADD8-6F2CF2468789");
        }

        /// <inheritdoc />
        public HttpServiceFixture<IAppointmentCommandService> HttpServiceFixture { get; }

        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task CancelAppointment_CommonSuccessfulIntegrationTest()
        {
            var appointmentId = Guid.Parse("01EED38A-5F7A-454D-8C72-83BBF324BCE2");
            const string cancelReason = "Some reason";
            var request = new CancelAppointmentRequestDto
            {
                Id = appointmentId,
                Reason = cancelReason
            };

            await _service.CancelAppointment(request);

            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(_ => _.Id == appointmentId);
            Assert.NotNull(appointment);
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.Equal(cancelReason, appointment.CancelReason);
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task CreateAppointment_CommonSuccessfulIntegrationTest()
        {
            var request = new CreateAppointmentRequestDto()
            {
                Title = "Test title",
                Description = "Description",
                StartDate = DateTime.UtcNow.AddHours(1),
                Duration = TimeSpan.FromHours(1),
                AppointmentType = API.Common.AppointmentType.Urgent,
                AttendeeIds = new[] { Guid.NewGuid(), Guid.NewGuid() }
            };

            var newItemId = await _service.CreateAppointment(request);

            var appointment = await _dbContext.Appointments.Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == newItemId);
            Assert.NotNull(appointment);
            Assert.Equal(request.Title, appointment.Title);
            Assert.Equal(request.Description, appointment.Description);
            Assert.Equal(request.Duration, appointment.Duration);
            Assert.Equal((int)request.AppointmentType, (int)appointment.Type);
            Assert.Equal(_currentUserId, appointment.CreatorId);
            Assert.Equal(request.AttendeeIds.Length, appointment.Attendees.Count);
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task RescheduleAppointment_CommonSuccessfulIntegrationTest()
        {
            var oldAppointment = await _dbContext.Appointments.AsNoTracking().Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.GetState() == AppointmentState.Opened);
            Assert.NotNull(oldAppointment);

            var request = new RescheduleAppointmentRequestDto()
            {
                StartDate = oldAppointment.StartDate,
                Duration = oldAppointment.Duration,
                Reason = "Test reason"
            };

            var newItemId = await _service.Reschedule(oldAppointment.Id, request);

            var newAppointment = await _dbContext.Appointments.Include(_ => _.Attendees)
                .FirstOrDefaultAsync(_ => _.Id == newItemId);
            Assert.NotNull(newAppointment);
            Assert.Equal(request.Duration, newAppointment.Duration);
            Assert.Equal(_currentUserId, newAppointment.CreatorId);
            Assert.Equal(AppointmentState.Opened, newAppointment.GetState());
        }
    }
}
