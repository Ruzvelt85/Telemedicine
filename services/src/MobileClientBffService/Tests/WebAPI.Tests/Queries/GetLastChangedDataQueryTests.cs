using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI;
using Telemedicine.Services.MobileClientBffService.WebAPI.Queries;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.UsersQueryService.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Queries
{
    public class GetLastChangedDataQueryTests
    {
        private static readonly Guid _currentUserId = Guid.NewGuid();

        private readonly Mock<IUsersQueryService> _usersQueryServiceApiMock;
        private readonly Mock<IMobileClientBffQueryService> _appointmentsBffQueryServiceApiMock;
        private readonly Mock<IMoodMeasurementQueryService> _moodMeasurementQueryServiceApiMock;
        private readonly GetLastChangedDataQueryHandler _queryHandler;

        public GetLastChangedDataQueryTests()
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            Mock<ICurrentUserProvider> currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _usersQueryServiceApiMock = new Mock<IUsersQueryService>();
            _appointmentsBffQueryServiceApiMock = new Mock<IMobileClientBffQueryService>();
            _moodMeasurementQueryServiceApiMock = new Mock<IMoodMeasurementQueryService>();

            currentUserProviderMock.Setup(_ => _.GetId()).Returns(_currentUserId);
            _usersQueryServiceApiMock.Setup(_ => _.GetUserInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(new UserInfoResponseDto(Guid.Empty, string.Empty, string.Empty, default));
            _appointmentsBffQueryServiceApiMock.Setup(_ => _.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(new AppointmentListResponseDto());
            _appointmentsBffQueryServiceApiMock.Setup(_ => _.GetChangedAppointmentList(It.IsAny<ChangedAppointmentListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(new AppointmentListResponseDto());
            _moodMeasurementQueryServiceApiMock.Setup(_ => _.GetMoodList(It.IsAny<GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(new PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>());

            _queryHandler = new GetLastChangedDataQueryHandler(mapper, currentUserProviderMock.Object, _usersQueryServiceApiMock.Object,
                _appointmentsBffQueryServiceApiMock.Object, _moodMeasurementQueryServiceApiMock.Object);
        }

        [Fact]
        public async Task HandleAsync_Default_ShouldReturnEmptyAppointmentsAndNullMood()
        {
            //Arrange
            var query = new GetLastChangedDataQuery();

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Appointments!);
            Assert.Null(result.Mood);

            _appointmentsBffQueryServiceApiMock.Verify(s => s.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), CancellationToken.None), Times.Once);
            _appointmentsBffQueryServiceApiMock.Verify(s => s.GetChangedAppointmentList(It.IsAny<ChangedAppointmentListRequestDto>(), CancellationToken.None), Times.Never);
            _usersQueryServiceApiMock.Verify(s => s.GetUserInfoAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never);
            _moodMeasurementQueryServiceApiMock.Verify(s => s.GetMoodList(It.IsAny<GetMeasurementListRequestDto>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnAppointmentsWithAttendees_WithoutCurrentUser()
        {
            //Arrange
            var userInfos = new List<UserInfoResponseDto>()
            {
                new(Guid.NewGuid(), "Alex", "Black", UserType.Patient),
                new(Guid.NewGuid(), "Andrew", "Brown", UserType.Doctor),
                new(_currentUserId, "John", "Green", UserType.Patient),
            };
            var appointments = new List<AppointmentResponseDto>()
            {
                GetAppointmentResponseDto(Guid.NewGuid(), "title1", DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(3), AppointmentState.Opened, AppointmentType.Urgent, false, _currentUserId, userInfos[0].Id),
                GetAppointmentResponseDto(Guid.NewGuid(), "title2", DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(3), AppointmentState.Opened, AppointmentType.Urgent, false, _currentUserId, userInfos[1].Id),
                GetAppointmentResponseDto(Guid.NewGuid(), "title3", DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(3), AppointmentState.Opened, AppointmentType.Urgent, false, _currentUserId, userInfos[0].Id, userInfos[1].Id),
            };

            _appointmentsBffQueryServiceApiMock.Setup(_ => _.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(new AppointmentListResponseDto { Appointments = appointments });
            _usersQueryServiceApiMock.Setup(_ => _.GetUserInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync((Guid id, CancellationToken _) => userInfos.FirstOrDefault(i => i.Id == id)!);

            var query = new GetLastChangedDataQuery();

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            _usersQueryServiceApiMock.Verify(s => s.GetUserInfoAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Exactly(2));
            _appointmentsBffQueryServiceApiMock.Verify(s => s.GetChangedAppointmentList(It.IsAny<ChangedAppointmentListRequestDto>(), CancellationToken.None), Times.Never);
            _appointmentsBffQueryServiceApiMock.Verify(s => s.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), CancellationToken.None), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(appointments.Count, result.Appointments!.Count);

            var appointment1 = result.Appointments.First(_ => _.Id == appointments[0].Id);
            Assert.NotNull(appointment1);
            Assert.Single(appointment1.Attendees);
            Assert.Equal(userInfos[0].Id, appointment1.Attendees.First().Id);

            var appointment2 = result.Appointments.First(_ => _.Id == appointments[1].Id);
            Assert.NotNull(appointment2);
            Assert.Single(appointment2.Attendees);
            Assert.Equal(userInfos[1].Id, appointment2.Attendees.First().Id);

            var appointment3 = result.Appointments.First(_ => _.Id == appointments[2].Id);
            Assert.NotNull(appointment3);
            Assert.Equal(appointments[2].Attendees.Count - 1, appointment3.Attendees.Count);
            Assert.Contains(appointment3.Attendees, a => a.Id == userInfos[0].Id);
            Assert.Contains(appointment3.Attendees, a => a.Id == userInfos[1].Id);
        }

        [Fact]
        public async Task HandleAsync_OnlyCurrentUser_ShouldReturnEmptyAppointmentsList()
        {
            //Arrange
            var appointments = new List<AppointmentResponseDto>()
            {
                GetAppointmentResponseDto(Guid.NewGuid(), "title1", DateTime.UtcNow.AddDays(2), TimeSpan.FromHours(3), AppointmentState.Opened, AppointmentType.Urgent, false, _currentUserId),
            };

            _appointmentsBffQueryServiceApiMock.Setup(_ => _.GetChangedAppointmentList(It.IsAny<ChangedAppointmentListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(new AppointmentListResponseDto { Appointments = appointments });

            var query = new GetLastChangedDataQuery() { AppointmentsLastUpdate = DateTime.UtcNow.AddHours(-2) };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            _usersQueryServiceApiMock.Verify(s => s.GetUserInfoAsync(It.IsAny<Guid>(), CancellationToken.None), Times.Never);
            _appointmentsBffQueryServiceApiMock.Verify(s => s.GetChangedAppointmentList(It.IsAny<ChangedAppointmentListRequestDto>(), CancellationToken.None), Times.Once);
            _appointmentsBffQueryServiceApiMock.Verify(s => s.GetAppointmentList(It.IsAny<AppointmentListRequestDto>(), CancellationToken.None), Times.Never);

            Assert.NotNull(result);
            Assert.Single(result.Appointments!);

            var appointment = result.Appointments!.First();
            Assert.Equal(appointments[0].Id, appointment.Id);
            Assert.Empty(appointment.Attendees);
        }

        [Fact]
        public async Task GetLastMoodList_WithoutMoodLastUpdate_ShouldReturnLastMood()
        {
            //Arrange
            var clientDate = new DateTime(2022, 1, 1, 10, 0, 0);
            var moodListResponse = new PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>
            {
                Items = new List<MeasurementResponseDto<MoodMeasurementDto>>
                {
                    new() { Id = Guid.NewGuid(), ClientDate = clientDate, Measure = new MoodMeasurementDto { Measure = MoodMeasureType.Happy } }
                }
            };

            _moodMeasurementQueryServiceApiMock.Setup(_ => _.GetMoodList(It.IsAny<GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(moodListResponse);

            var query = new GetLastChangedDataQuery { MoodLastUpdate = null };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Mood);
            Assert.Equal(clientDate, result.Mood!.ClientDate);
        }

        [Fact]
        public async Task GetLastMoodList_WithoutEmptyMoodItems_ShouldReturnNull()
        {
            //Arrange
            var moodListResponse = new PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>
            {
                Items = new List<MeasurementResponseDto<MoodMeasurementDto>>()
            };

            _moodMeasurementQueryServiceApiMock.Setup(_ => _.GetMoodList(It.IsAny<GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(moodListResponse);

            var query = new GetLastChangedDataQuery { MoodLastUpdate = null };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Mood);
        }

        [Fact]
        public async Task GetLastMoodList_WithMoodLastUpdate_ShouldReturnMood()
        {
            //Arrange
            var clientDate = new DateTime(2022, 1, 1, 10, 0, 0);
            var moodListResponse = new PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>
            {
                Items = new List<MeasurementResponseDto<MoodMeasurementDto>>
                {
                    new() { Id = Guid.NewGuid(), ClientDate = clientDate, Measure = new MoodMeasurementDto { Measure = MoodMeasureType.Happy } }
                }
            };
            _moodMeasurementQueryServiceApiMock.Setup(_ => _.GetMoodList(It.IsAny<GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(moodListResponse);

            var query = new GetLastChangedDataQuery { MoodLastUpdate = clientDate.AddSeconds(-1) };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Mood);
            Assert.Equal(clientDate, result.Mood!.ClientDate);
        }

        [Fact]
        public async Task GetLastMoodList_WithMoodLastUpdateSameAsClientDate_ShouldReturnNull()
        {
            //Arrange
            var clientDate = new DateTime(2022, 1, 1, 10, 20, 50);
            var moodListResponse = new PagedListResponseDto<MeasurementResponseDto<MoodMeasurementDto>>
            {
                Items = new List<MeasurementResponseDto<MoodMeasurementDto>>
                {
                    new() { Id = Guid.NewGuid(), ClientDate = clientDate, Measure = new MoodMeasurementDto { Measure = MoodMeasureType.Happy } }
                }
            };
            _moodMeasurementQueryServiceApiMock.Setup(_ => _.GetMoodList(It.IsAny<GetMeasurementListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(moodListResponse);

            var query = new GetLastChangedDataQuery { MoodLastUpdate = clientDate };

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Mood);
        }

        private static AppointmentResponseDto GetAppointmentResponseDto(Guid id, string title, DateTime startDate, TimeSpan duration, AppointmentState state, AppointmentType type, bool isDeleted, params Guid[] attendeeIds)
        {
            return new AppointmentResponseDto(id, title, startDate, duration, state, type, DateTime.UtcNow, isDeleted) { Attendees = attendeeIds };
        }
    }
}
