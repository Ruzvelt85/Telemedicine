using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.MobileClientBffService.API;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class MobileClientQueryServiceTests : IHttpServiceTests<IMobileClientQueryService>
    {
        private readonly IMobileClientQueryService _service;

        /// <inheritdoc />
        public HttpServiceFixture<IMobileClientQueryService> HttpServiceFixture { get; }

        public MobileClientQueryServiceTests(HttpServiceFixture<IMobileClientQueryService> httpServiceFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            _service = HttpServiceFixture.GetRestService();
        }

        /// <summary>
        /// Service should return not deleted appointments related to PatientId = "C3F1B4E2-4C72-437D-933F-879E031DC629"
        /// For test purpose we use here existing ID from data seed
        /// </summary>
        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetLastChangedData_ShouldReturnNotDeletedAppointments()
        {
            var request = new LastChangedDataRequestDto()
            {
                AppointmentsLastUpdate = null
            };

            var response = await _service.GetLastChangedData(request);

            Assert.NotNull(response);
            Assert.NotNull(response.Appointments);
            Assert.Equal(252, response.Appointments!.Count);
            Assert.DoesNotContain(response.Appointments, _ => _.Id == Guid.Parse("AE07E843-F3F6-43E0-8E07-4172C8319BCA")); // deleted appointment

            var appointment1 = response.Appointments.FirstOrDefault(_ => _.Id == Guid.Parse("01EED38A-5F7A-454D-8C72-83BBF324BCE2"));
            Assert.NotNull(appointment1);
            Assert.Equal("Call about next check-up Bill Tryck", appointment1!.Title);
            Assert.Equal(new DateTime(2021, 11, 19), appointment1.StartDate);
            Assert.Equal(TimeSpan.FromMinutes(45), appointment1.Duration);
            Assert.Equal("FollowUp", appointment1.Type.ToString());
            Assert.Equal("Opened", appointment1.Status.ToString());

            Assert.NotNull(appointment1.Attendees);
            Assert.Equal(1, appointment1.Attendees.Count);

            // should not return current user
            Assert.DoesNotContain(appointment1.Attendees, _ => _.Id == Guid.Parse("C3F1B4E2-4C72-437D-933F-879E031DC629"));

            var doctorId = Guid.Parse("AAB4ED21-0933-4D1F-ADD8-6F2CF2468789");
            var doctor = appointment1.Attendees.FirstOrDefault(a => a.Id == doctorId);
            Assert.NotNull(doctor);
            Assert.Equal(UserType.Doctor.ToString(), doctor!.UserType.ToString());
            Assert.Equal("Angelina", doctor.FirstName);
            Assert.Equal("Filding", doctor.LastName);
        }

        /// <summary>
        /// Service method should return changed appointments (added, modified, deleted) since AppointmentsLastUpdate
        /// Due to AppointmentsLastUpdate equals yesterday, this method should return all existing appointments for current patient
        /// For test purpose we use here existing ID from data seed (PatientId = "C3F1B4E2-4C72-437D-933F-879E031DC629")
        /// </summary>
        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetLastChangedData_ShouldReturnChangedAppointments()
        {
            var request = new LastChangedDataRequestDto()
            {
                AppointmentsLastUpdate = DateTime.UtcNow.AddDays(-1),
            };

            var response = await _service.GetLastChangedData(request);

            Assert.NotNull(response);
            Assert.NotNull(response.Appointments);
            Assert.Equal(253, response.Appointments!.Count);
            Assert.Contains(response.Appointments, _ => _.Id == Guid.Parse("01EED38A-5F7A-454D-8C72-83BBF324BCE2"));

            var appointment1 = response.Appointments.FirstOrDefault(_ => _.Id == Guid.Parse("AE07E843-F3F6-43E0-8E07-4172C8319BCA"));
            Assert.NotNull(appointment1);
            Assert.Equal("Deleted Call about next check-up Bill Tryck", appointment1!.Title);
            Assert.Equal(new DateTime(2021, 11, 19), appointment1.StartDate);
            Assert.Equal(TimeSpan.FromMinutes(45), appointment1.Duration);
            Assert.Equal("Annual", appointment1.Type.ToString());
            Assert.Equal("Cancelled", appointment1.Status.ToString());
            Assert.True(appointment1.IsDeleted);

            Assert.NotNull(appointment1.Attendees);
            Assert.Equal(1, appointment1.Attendees.Count);

            // should not return current user
            Assert.DoesNotContain(appointment1.Attendees, _ => _.Id == Guid.Parse("C3F1B4E2-4C72-437D-933F-879E031DC629"));

            var doctorId = Guid.Parse("AAB4ED21-0933-4D1F-ADD8-6F2CF2468789");
            var doctor = appointment1.Attendees.FirstOrDefault(a => a.Id == doctorId);
            Assert.NotNull(doctor);
            Assert.Equal(UserType.Doctor.ToString(), doctor!.UserType.ToString());
            Assert.Equal("Angelina", doctor.FirstName);
            Assert.Equal("Filding", doctor.LastName);
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetLastChangedData_ShouldReturnLastMood()
        {
            var request = new LastChangedDataRequestDto
            {
                MoodLastUpdate = DateTime.Parse("2021-11-01"),
            };

            var response = await _service.GetLastChangedData(request);

            Assert.NotNull(response);
            Assert.NotNull(response.Mood);
            Assert.Equal(MoodMeasureType.Sad, response.Mood!.Measure);
        }
    }
}
