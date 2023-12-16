using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Moq;
using Xunit;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Appointments;
using AppointmentServiceDto = Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.Appointments
{
    public class GetAppointmentListQueryHandlerTests
    {
        private readonly Mock<ICurrentUserProvider> _currentUserProviderMock;

        private readonly Mock<IUsersQueryService> _usersQueryServiceApiMock;

        private readonly Mock<IWebClientBffQueryService> _appointmentsQueryServiceApiMock;

        private readonly GetAppointmentListQueryHandler _queryHandler;

        private static readonly Guid _doctorId = Guid.NewGuid();

        private static readonly Guid[] _patientIds = { Guid.NewGuid(), Guid.NewGuid() };

        private readonly Guid[] _appointmentIds = { Guid.NewGuid(), Guid.NewGuid() };

        private readonly Guid[] _attendeeIds = { _doctorId, _patientIds[0], _patientIds[1] };

        public GetAppointmentListQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _usersQueryServiceApiMock = new Mock<IUsersQueryService>();
            _appointmentsQueryServiceApiMock = new Mock<IWebClientBffQueryService>();

            _queryHandler = new GetAppointmentListQueryHandler(mapper,
                _currentUserProviderMock.Object,
                _usersQueryServiceApiMock.Object,
                _appointmentsQueryServiceApiMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnAppointmentsByUserId()
        {
            //Arrange
            MockServicesData(_doctorId);

            var filterRequest = new AppointmentListFilterRequestDto
            {
                AppointmentStatus = AppointmentStatus.Opened,
                DateRange = new Range<DateTime?>(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1))
            };
            var query = new GetAppointmentListQuery(filterRequest, new PagingRequestDto());

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            _appointmentsQueryServiceApiMock.Verify(_ => _.GetAppointmentList(It.Is<AppointmentServiceDto.AppointmentListRequestDto>(dto => dto.Filter.AttendeeId == _doctorId), CancellationToken.None), Times.Once);
            _usersQueryServiceApiMock.Verify(_ => _.GetUserInfoDetailsAsync(It.Is<Guid>(id => _attendeeIds.Contains(id)), CancellationToken.None), Times.Exactly(3));

            Assert.NotNull(result);
            Assert.NotNull(result.Paging);
            Assert.Equal(2, result.Paging!.Total);

            Assert.NotNull(result.Items);
            Assert.Equal(2, result.Items!.Count);

            var appointment1 = result.Items.FirstOrDefault(_ => _.Id == _appointmentIds[0]);
            Assert.NotNull(appointment1);
            Assert.NotNull(appointment1!.Attendees);
            Assert.Equal(2, appointment1.Attendees!.Count);
            Assert.Contains(appointment1.Attendees, _ => _.Id == _doctorId);
            Assert.Contains(appointment1.Attendees, _ => _.Id == _patientIds[0]);

            var appointment2 = result.Items.FirstOrDefault(_ => _.Id == _appointmentIds[1]);
            Assert.NotNull(appointment2);
            Assert.NotNull(appointment2!.Attendees);
            Assert.Equal(2, appointment2.Attendees!.Count);
            Assert.Contains(appointment2.Attendees, _ => _.Id == _doctorId);
            Assert.Contains(appointment2.Attendees, _ => _.Id == _patientIds[1]);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnEmptyResult()
        {
            //Arrange
            MockServicesData(Guid.NewGuid()); // will return empty items
            var filterRequest = new AppointmentListFilterRequestDto
            {
                AppointmentStatus = AppointmentStatus.Opened,
                DateRange = new Range<DateTime?>(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1))
            };
            var query = new GetAppointmentListQuery(filterRequest, new PagingRequestDto());

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            _appointmentsQueryServiceApiMock.Verify(_ => _.GetAppointmentList(It.Is<AppointmentServiceDto.AppointmentListRequestDto>(dto => dto.Filter.AttendeeId == _doctorId), CancellationToken.None), Times.Once);
            _usersQueryServiceApiMock.Verify(_ => _.GetUserInfoDetailsAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never);

            Assert.NotNull(result);
            Assert.NotNull(result.Paging);
            Assert.Equal(0, result.Paging!.Total);

            Assert.NotNull(result.Items);
            Assert.Empty(result.Items!);
        }

        private void MockServicesData(Guid attendeeId)
        {
            _currentUserProviderMock.Setup(m => m.GetId()).Returns(_doctorId);

            var appointments = new List<AppointmentServiceDto.AppointmentResponseDto>
            {
                new()
                {
                    Id = _appointmentIds[0], Title = "title", State = AppointmentState.Opened, StartDate = new DateTime(2020, 4, 4),
                    Attendees = new[] { _doctorId, _patientIds[0] }
                },
                new()
                {
                    Id = _appointmentIds[1], Title = "title 2", State = AppointmentState.Opened, StartDate = new DateTime(2020, 5, 5),
                    Attendees = new[] { _doctorId, _patientIds[1] }
                }
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
            _usersQueryServiceApiMock.Setup(_ => _.GetUserInfoDetailsAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((Guid id, CancellationToken _) => userInfos.FirstOrDefault(i => i.Id == id)!);
        }

    }
}
