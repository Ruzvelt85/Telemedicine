using System;
using System.Linq;
using System.Threading.Tasks;
using Refit;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.IntegrationTesting;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.API.Common;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.IntegrationTests
{
    [Trait("Category", "Integration")]
    [Collection("Integration")]
    public class AppointmentsQueryIntegrationTests : IHttpServiceTests<IAppointmentQueryService>, IDbContextTests<AppointmentDomainServiceDbContext>
    {
        private readonly IAppointmentQueryService _service;

        public AppointmentsQueryIntegrationTests(HttpServiceFixture<IAppointmentQueryService> httpServiceFixture, EfCoreContextFixture<AppointmentDomainServiceDbContext> efCoreContextFixture)
        {
            HttpServiceFixture = httpServiceFixture;
            EfCoreContextFixture = efCoreContextFixture;
            _service = HttpServiceFixture.GetRestService();
        }

        /// <inheritdoc />
        public HttpServiceFixture<IAppointmentQueryService> HttpServiceFixture { get; }

        public EfCoreContextFixture<AppointmentDomainServiceDbContext> EfCoreContextFixture { get; }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetAppointmentList_CommonSuccessfulIntegrationTest()
        {
            // For test purpose we use here existing ID from data seed
            // We cannot create new test data in databases due to need of isolation and other restrictions
            //Guid doctorId = Guid.Parse("AAB4ED21-0933-4D1F-ADD8-6F2CF2468789");

            var request = new AppointmentListRequestDto
            {
                Paging = new PagingRequestDto(),
                Filter = new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(new DateTime(2021, 10, 1), new DateTime(2022, 10, 1)),
                }
            };

            AppointmentListResponseDto appointmentResponse = await _service.GetAppointmentList(request);

            Assert.NotNull(appointmentResponse);
            Assert.NotNull(appointmentResponse.Paging);
            Assert.NotNull(appointmentResponse.Items);
            Assert.NotEmpty(appointmentResponse.Items!);

            var appointment = appointmentResponse.Items!.FirstOrDefault();

            Assert.NotNull(appointment);
            Assert.NotNull(appointment!.Attendees);
            Assert.NotEmpty(appointment.Attendees!);
        }

        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetAppointmentById_CommonSuccessfulIntegrationTest()
        {
            Guid appointmentId = Guid.Parse("01EED38A-5F7A-454D-8C72-83BBF324BCE2");

            var result = await _service.GetAppointmentByIdAsync(appointmentId);

            Assert.NotNull(result);
            Assert.Equal(appointmentId, result.Id);
            Assert.Equal("Call about next check-up Bill Tryck", result.Title);
            Assert.Null(result.Description);
            Assert.Equal(new DateTime(2021, 11, 19), result.StartDate);
            Assert.Equal(TimeSpan.FromMinutes(45), result.Duration);
            Assert.Equal("FollowUp", result.Type.ToString());
            Assert.Equal("Opened", result.Status.ToString());
            Assert.Equal(0, result.Rating);

            Assert.NotNull(result.Attendees);
            Assert.Equal(2, result.Attendees.Count);

            var patientId = Guid.Parse("C3F1B4E2-4C72-437D-933F-879E031DC629");
            var patient = result.Attendees.FirstOrDefault(a => a.Id == patientId);
            Assert.NotNull(patient);
            Assert.Equal(UserType.Patient.ToString(), patient!.UserType.ToString());
            Assert.Equal("Bill", patient.FirstName);
            Assert.Equal("Tryck", patient.LastName);

            var doctorId = Guid.Parse("AAB4ED21-0933-4D1F-ADD8-6F2CF2468789");
            var doctor = result.Attendees.FirstOrDefault(a => a.Id == doctorId);
            Assert.NotNull(doctor);
            Assert.Equal(UserType.Doctor.ToString(), doctor!.UserType.ToString());
            Assert.Equal("Angelina", doctor.FirstName);
            Assert.Equal("Filding", doctor.LastName);
        }

        /// <summary>
        /// For test purpose we use here existing ID from data seed (AppointmentId = "C3F1B4E2-4C72-437D-933F-879E031DC629")
        /// </summary>
        [Fact(Skip = "Test uses seeds; must be launched after data seeding")]
        public async Task GetConnectionInfo_ShouldReturnCorrectException()
        {
            await Assert.ThrowsAsync<ApiException>(() => _service.GetAppointmentConnectionInfoAsync(Guid.Parse("A85475D6-3364-4E2C-B1F7-0863469EFB88")));
        }
    }
}
