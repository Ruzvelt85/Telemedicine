using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments;
using AppointmentType = Telemedicine.Services.AppointmentDomainService.API.Common.Common.AppointmentType;
using UserType = Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.UserType;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.Appointments
{
    public class GetAppointmentByIdQueryHandlerTests
    {
        private readonly Mock<IAppointmentQueryService> _appointmentsQueryServiceApiMock;
        private readonly Mock<IUsersQueryService> _usersQueryServiceApiMock;
        private readonly GetAppointmentByIdQueryHandler _queryHandler;

        public GetAppointmentByIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
            _appointmentsQueryServiceApiMock = new Mock<IAppointmentQueryService>();
            _usersQueryServiceApiMock = new Mock<IUsersQueryService>();

            _queryHandler = new GetAppointmentByIdQueryHandler(mapper, _appointmentsQueryServiceApiMock.Object, _usersQueryServiceApiMock.Object);
        }

        [Fact]
        public async Task HandleAsync_NoAttendees_ShouldReturnAppointmentWithoutAttendees()
        {
            //Arrange
            var appointmentId = Guid.NewGuid();
            var appointmentResponseDto = new AppointmentByIdResponseDto(appointmentId, "Appointment", "Description of appointment",
                new DateTime(2022, 1, 20), TimeSpan.FromMinutes(45), AppointmentType.Annual, AppointmentState.Opened, 0);

            _appointmentsQueryServiceApiMock.Setup(_ => _.GetAppointmentByIdAsync(appointmentId, CancellationToken.None))
                .ReturnsAsync(appointmentResponseDto);

            var query = new GetAppointmentByIdQuery(appointmentId);

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentResponseDto.Id, result.Id);
            Assert.Equal(appointmentResponseDto.Title, result.Title);
            Assert.Equal(appointmentResponseDto.Description, result.Description);
            Assert.Equal(appointmentResponseDto.StartDate, result.StartDate);
            Assert.Equal(appointmentResponseDto.Duration, result.Duration);
            Assert.Equal(appointmentResponseDto.Type.ToString(), result.Type.ToString());

            Assert.Equal(appointmentResponseDto.State.ToString(), API.Common.AppointmentStatus.Opened.ToString());
            Assert.Equal(appointmentResponseDto.Rating, result.Rating);

            Assert.NotNull(result.Attendees);
            Assert.Empty(result.Attendees);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnAppointmentWithAttendees()
        {
            //Arrange
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var appointmentId = Guid.NewGuid();
            var appointmentResponseDto = new AppointmentByIdResponseDto(appointmentId, "Appointment", "Description of appointment",
                new DateTime(2022, 1, 20), TimeSpan.FromMinutes(45), AppointmentType.Annual, AppointmentState.Opened, 0)
            { Attendees = userIds };

            var patientInfo = new PatientInfoResponseDto(userIds[0], "Alex", "Black", UserType.Patient, "innerId", "1234567", new DateTime(1950, 1, 1));
            var doctorInfo = new DoctorInfoResponseDto(userIds[1], "Andrew", "Brown", UserType.Doctor, "innerId2", "2345678");
            var userInfos = new List<UserInfoDetailsResponseDto> { patientInfo, doctorInfo };

            _appointmentsQueryServiceApiMock.Setup(_ => _.GetAppointmentByIdAsync(appointmentId, CancellationToken.None))
                .ReturnsAsync(appointmentResponseDto);

            _usersQueryServiceApiMock.Setup(_ => _.GetUserInfoDetailsAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(new Func<Guid, CancellationToken, UserInfoDetailsResponseDto>((id, _) => userInfos.FirstOrDefault(info => info.Id == id)!));

            var query = new GetAppointmentByIdQuery(appointmentId);

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointmentResponseDto.Id, result.Id);
            Assert.Equal(appointmentResponseDto.Title, result.Title);
            Assert.Equal(appointmentResponseDto.Description, result.Description);
            Assert.Equal(appointmentResponseDto.StartDate, result.StartDate);
            Assert.Equal(appointmentResponseDto.Duration, result.Duration);
            Assert.Equal(appointmentResponseDto.Type.ToString(), result.Type.ToString());
            Assert.Equal(API.Common.AppointmentStatus.Opened.ToString(), result.Status.ToString());
            Assert.Equal(appointmentResponseDto.Rating, result.Rating);

            Assert.NotNull(result.Attendees);
            Assert.Equal(userInfos.Count, result.Attendees.Count);

            var patient = result.Attendees.First(a => a.Id == userInfos[0].Id);
            Assert.NotNull(patient);
            Assert.Equal(patientInfo.Type.ToString(), patient.UserType.ToString());
            Assert.Equal(patientInfo.FirstName, patient.FirstName);
            Assert.Equal(patientInfo.LastName, patient.LastName);

            var doctor = result.Attendees.FirstOrDefault(a => a.Id == userInfos[1].Id);
            Assert.NotNull(doctor);
            Assert.Equal(doctorInfo.Type.ToString(), doctor!.UserType.ToString());
            Assert.Equal(doctorInfo.FirstName, doctor.FirstName);
            Assert.Equal(doctorInfo.LastName, doctor.LastName);
        }
    }
}
