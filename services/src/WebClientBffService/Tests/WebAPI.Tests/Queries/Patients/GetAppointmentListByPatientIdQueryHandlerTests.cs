using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetAppointmentListByPatientId;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients;
using AppointmentServiceDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.Patients
{
    public class GetAppointmentListByPatientIdQueryHandlerTests
    {
        private readonly Mock<ICheckAccessProvider> _checkAccessProviderMock;

        private readonly Mock<ICurrentUserProvider> _currentUserProviderMock;

        private readonly Mock<IUsersQueryService> _usersQueryServiceApiMock;

        private readonly Mock<IPatientsQueryService> _patientsQueryServiceApiMock;

        private readonly Mock<IWebClientBffQueryService> _appointmentsQueryServiceApiMock;

        private readonly GetAppointmentListByPatientIdQueryHandler _queryHandler;

        private static readonly Guid _doctorId = Guid.NewGuid();

        private static readonly Guid[] _patientIds = { Guid.NewGuid(), Guid.NewGuid() };

        private readonly Guid[] _appointmentIds = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        private readonly Guid[] _attendeeIds = { _doctorId, _patientIds[0], _patientIds[1] };

        public GetAppointmentListByPatientIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
            _checkAccessProviderMock = new Mock<ICheckAccessProvider>();
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _usersQueryServiceApiMock = new Mock<IUsersQueryService>();
            _patientsQueryServiceApiMock = new Mock<IPatientsQueryService>();
            _appointmentsQueryServiceApiMock = new Mock<IWebClientBffQueryService>();

            _queryHandler = new GetAppointmentListByPatientIdQueryHandler(mapper,
                _checkAccessProviderMock.Object,
                _currentUserProviderMock.Object,
                _usersQueryServiceApiMock.Object,
                _patientsQueryServiceApiMock.Object,
                _appointmentsQueryServiceApiMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnAppointmentsByPatientId()
        {
            //Arrange
            var patientId = _patientIds[1];
            MockServicesData(patientId);
            var filterRequest = new AppointmentListByPatientIdFilterRequestDto
            {
                AppointmentStatus = AppointmentStatus.Opened,
                DateRange = new Range<DateTime?>(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1))
            };
            var query = new GetAppointmentListByPatientIdQuery(filterRequest, new PagingRequestDto()) { PatientId = patientId };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            _patientsQueryServiceApiMock.Verify(_ => _.GetPatientByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _checkAccessProviderMock.Verify(_ => _.ShouldHaveSameHealthCenterAsync(It.IsAny<Guid[]>()), Times.Once);
            _appointmentsQueryServiceApiMock.Verify(_ => _.GetAppointmentList(It.Is<AppointmentServiceDto.AppointmentListRequestDto>(dto => dto.Filter.AttendeeId == patientId), CancellationToken.None), Times.Once);
            _usersQueryServiceApiMock.Verify(_ => _.GetUserInfoAsync(It.Is<Guid>(id => _attendeeIds.Contains(id)), CancellationToken.None), Times.Exactly(2));

            Assert.NotNull(result);
            Assert.NotNull(result.Paging);
            Assert.Equal(1, result.Paging.Total);

            Assert.NotNull(result.Items);
            Assert.Single(result.Items);

            var appointment = result.Items.First(_ => _.Id == _appointmentIds[1]);
            Assert.NotNull(appointment);
            Assert.NotNull(appointment.Attendees);
            Assert.Equal(2, appointment.Attendees.Count);
            Assert.Contains(appointment.Attendees, _ => _.Id == _doctorId);
            Assert.Contains(appointment.Attendees, _ => _.Id == patientId);
        }

        [Fact]
        public async Task HandleAsync_ShouldInvokeUsersQueryServiceOnlyTwice()
        {
            //Arrange
            var patientId = _patientIds[0];
            MockServicesData(patientId);
            var filterRequest = new AppointmentListByPatientIdFilterRequestDto
            {
                AppointmentStatus = AppointmentStatus.Opened,
                DateRange = new Range<DateTime?>(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1))
            };
            var query = new GetAppointmentListByPatientIdQuery(filterRequest, new PagingRequestDto()) { PatientId = patientId };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            _patientsQueryServiceApiMock.Verify(_ => _.GetPatientByIdAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Once);
            _checkAccessProviderMock.Verify(_ => _.ShouldHaveSameHealthCenterAsync(It.IsAny<Guid[]>()), Times.Once);
            _appointmentsQueryServiceApiMock.Verify(_ => _.GetAppointmentList(It.Is<AppointmentServiceDto.AppointmentListRequestDto>(dto => dto.Filter.AttendeeId == patientId), CancellationToken.None), Times.Once);
            _usersQueryServiceApiMock.Verify(_ => _.GetUserInfoAsync(It.Is<Guid>(id => _attendeeIds.Contains(id)), CancellationToken.None), Times.Exactly(2));

            Assert.NotNull(result);
            Assert.NotNull(result.Paging);
            Assert.Equal(2, result.Paging.Total);

            Assert.NotNull(result.Items);
            Assert.Equal(2, result.Items.Count);

            var appointment1 = result.Items.FirstOrDefault(_ => _.Id == _appointmentIds[0]);
            Assert.NotNull(appointment1);
            Assert.NotNull(appointment1!.Attendees);
            Assert.Equal(2, appointment1.Attendees.Count);
            Assert.Contains(appointment1.Attendees, _ => _.Id == _doctorId);
            Assert.Contains(appointment1.Attendees, _ => _.Id == patientId);

            var appointment2 = result.Items.First(_ => _.Id == _appointmentIds[0]);
            Assert.NotNull(appointment2);
            Assert.NotNull(appointment2.Attendees);
            Assert.Equal(2, appointment2.Attendees.Count);
            Assert.Contains(appointment2.Attendees, _ => _.Id == _doctorId);
            Assert.Contains(appointment2.Attendees, _ => _.Id == patientId);
        }

        private void MockServicesData(Guid attendeeId)
        {
            _currentUserProviderMock.Setup(m => m.GetId()).Returns(_doctorId);

            _checkAccessProviderMock.Setup(m => m.ShouldHaveSameHealthCenterAsync(It.IsAny<Guid[]>())).Returns(Task.CompletedTask);

            var appointments = new List<AppointmentServiceDto.AppointmentResponseDto>
            {
                new()
                {
                    Id = _appointmentIds[0], Title = "title", State = AppointmentState.Opened, StartDate = new DateTime(2021, 4, 4),
                    Attendees = new[] { _doctorId, _patientIds[0] }
                },
                new()
                {
                    Id = _appointmentIds[1], Title = "title 2", State = AppointmentState.Opened, StartDate = new DateTime(2021, 5, 5),
                    Attendees = new[] { _doctorId, _patientIds[1] }
                },
                new()
                {
                    Id = _appointmentIds[2], Title = "title 3", State = AppointmentState.Opened, StartDate = new DateTime(2021, 6, 6),
                    Attendees = new[] { _doctorId, _patientIds[0] }
                },
            };
            var items = appointments.Where(_ => _.Attendees.Contains(attendeeId)).ToArray();
            var appointmentListResponse = new AppointmentServiceDto.AppointmentListResponseDto
            {
                Items = items,
                Paging = new PagingResponseDto(items.Length)
            };
            _appointmentsQueryServiceApiMock.Setup(_ => _.GetAppointmentList(It.IsAny<AppointmentServiceDto.AppointmentListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(appointmentListResponse);

            var patientInfo1 = new PatientInfoResponseDto(_patientIds[0], "Alex", "Black", HealthCenterStructureDomainService.API.Common.Common.UserType.Patient, "innerId", "1234567", new DateTime(1950, 1, 1));
            var patientInfo2 = new PatientInfoResponseDto(_patientIds[1], "Max", "Green", HealthCenterStructureDomainService.API.Common.Common.UserType.Patient, "innerId2", "7654321", new DateTime(1940, 10, 10));
            var doctorInfo = new DoctorInfoResponseDto(_doctorId, "Andrew", "Brown", HealthCenterStructureDomainService.API.Common.Common.UserType.Doctor, "innerId3", "2345678");
            var userInfos = new List<UserInfoDetailsResponseDto> { patientInfo1, patientInfo2, doctorInfo };
            _usersQueryServiceApiMock.Setup(_ => _.GetUserInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((Guid id, CancellationToken _) => userInfos.FirstOrDefault(i => i.Id == id)!);
        }

    }
}
