using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Dto;
using Telemedicine.Common.Business.BusinessLogic.ConferenceConnectionInfo.Exceptions;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Queries
{
    public class GetAppointmentConnectionInfoQueryHandlerTests
    {
        private readonly GetAppointmentConnectionInfoQueryHandler _queryHandler;
        private readonly Mock<IAppointmentConnectionInfoProvider> _appointmentConnectionInfoProviderMock;
        private readonly Guid _currentUserId = Guid.NewGuid();

        private readonly AppointmentConnectionInfoResponseDto _testConnectionInfoData = new()
        {
            Id = Guid.NewGuid(),
            RoomId = 123,
            Host = "http://test.com",
            RoomKey = "thdjvlx",
            RoomPin = "111"
        };

        public GetAppointmentConnectionInfoQueryHandlerTests()
        {
            Mock<ICurrentUserProvider> currentUserProviderMock = new();
            currentUserProviderMock.Setup(m => m.GetId()).Returns(_currentUserId);

            _appointmentConnectionInfoProviderMock = new Mock<IAppointmentConnectionInfoProvider>();
            _appointmentConnectionInfoProviderMock
                .Setup(_ => _.GetConnectionInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(_testConnectionInfoData);

            var videoConferenceSettingsMock = new Mock<IOptionsSnapshot<VideoConferenceConnectionSettings>>();
            videoConferenceSettingsMock.Setup(m => m.Value)
                .Returns(new VideoConferenceConnectionSettings { TimeInSecondsBeforeAppointmentStartWhenGettingConnectionInfoAllowed = 60 });

            _queryHandler = new GetAppointmentConnectionInfoQueryHandler(
                videoConferenceSettingsMock.Object,
                currentUserProviderMock.Object,
                _appointmentConnectionInfoProviderMock.Object);
        }

        [Fact]
        public async Task HandleAsync_GetAppointmentConnectionInfo_ShouldReturnCorrectConnectionInfo()
        {
            //Arrange
            var appointmentId = Guid.NewGuid();
            var utcNow = DateTime.UtcNow;

            var appointmentResponseDto = new AppointmentInfoDto
            {
                Id = appointmentId,
                StartDate = utcNow,
                Duration = TimeSpan.FromHours(1),
                Attendees = new List<Guid>() { _currentUserId }
            };

            _appointmentConnectionInfoProviderMock
                .Setup(_ => _.GetAppointmentInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(appointmentResponseDto);

            var query = new GetAppointmentConnectionInfoQuery(appointmentId);

            //Act
            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testConnectionInfoData.Id, result.Id);
            Assert.Equal(_testConnectionInfoData.RoomId, result.RoomId);
            Assert.Equal(_testConnectionInfoData.Host, result.Host);
            Assert.Equal(_testConnectionInfoData.RoomKey, result.RoomKey);
            Assert.Equal(_testConnectionInfoData.RoomPin, result.RoomPin);
        }

        [Fact]
        public async Task HandleAsync_GetAppointmentConnectionInfo_ShouldThrowTooEarlyException()
        {
            //Arrange
            var appointmentId = Guid.NewGuid();
            var utcNow = DateTime.UtcNow;
            var dateInHour = utcNow.AddHours(1);

            var appointmentInHour = new AppointmentInfoDto
            {
                Id = appointmentId,
                StartDate = dateInHour,
                Duration = TimeSpan.FromHours(1),
                Attendees = new List<Guid>() { _currentUserId }
            };

            _appointmentConnectionInfoProviderMock
                .Setup(_ => _.GetAppointmentInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(appointmentInHour);

            var query = new GetAppointmentConnectionInfoQuery(appointmentId);

            //Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<AppointmentConnectionInfoRequestedTooEarlyException>(exception);

            var customException = exception as AppointmentConnectionInfoRequestedTooEarlyException;
            Assert.NotNull(customException);
            Assert.Equal(appointmentInHour.StartDate, customException!.StartDate);
            Assert.Equal(AppointmentConnectionInfoBusinessException.ErrorType.TooEarlyToRequestConnectionInfo.ToString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_GetAppointmentConnectionInfo_ShouldThrowTooLateException()
        {
            //Arrange
            var appointmentId = Guid.NewGuid();
            var utcNow = DateTime.UtcNow;
            var conferenceDuration = TimeSpan.FromHours(1);
            var pastConferenceDate = utcNow.AddTicks(-conferenceDuration.Ticks).AddTicks(-1);

            var appointmentResponseDto = new AppointmentInfoDto
            {
                Id = appointmentId,
                StartDate = pastConferenceDate,
                Duration = conferenceDuration,
                Attendees = new List<Guid>() { _currentUserId }
            };

            _appointmentConnectionInfoProviderMock
                .Setup(_ => _.GetAppointmentInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(appointmentResponseDto);

            var query = new GetAppointmentConnectionInfoQuery(appointmentId);

            //Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<AppointmentConnectionInfoRequestedTooLateException>(exception);

            var customException = exception as AppointmentConnectionInfoRequestedTooLateException;
            Assert.NotNull(customException);
            Assert.Equal(appointmentResponseDto.StartDate, customException!.StartDate);
            Assert.Equal(AppointmentConnectionInfoBusinessException.ErrorType.TooLateToRequestConnectionInfo.ToString(), customException.Code);
        }

        [Fact]
        public async Task HandleAsync_GetAppointmentConnectionInfo_ShouldThrowRequestedByWrongUserException()
        {
            var appointmentId = Guid.NewGuid();
            var utcNow = DateTime.UtcNow;

            var appointmentResponseDto = new AppointmentInfoDto
            {
                Id = appointmentId,
                StartDate = utcNow,
                Duration = TimeSpan.FromHours(1),
                Attendees = new List<Guid>() { Guid.NewGuid() }
            };

            _appointmentConnectionInfoProviderMock
                .Setup(_ => _.GetAppointmentInfoAsync(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(appointmentResponseDto);

            var query = new GetAppointmentConnectionInfoQuery(appointmentId);

            //Act
            var exception = await Record.ExceptionAsync(() => _queryHandler.HandleAsync(query));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<AppointmentConnectionInfoRequestedByWrongUserException>(exception);

            var customException = exception as AppointmentConnectionInfoRequestedByWrongUserException;
            Assert.NotNull(customException);
            Assert.Equal(appointmentResponseDto.Id, customException!.AppointmentId);
            Assert.Equal(AppointmentConnectionInfoBusinessException.ErrorType.ConnectionInfoRequestedByWrongUser.ToString(), customException.Code);
        }
    }
}
