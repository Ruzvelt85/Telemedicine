using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService;
using Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentCommandService.Dto;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.WebAPI;
using Telemedicine.Services.WebClientBffService.WebAPI.Commands.Appointments;
using Moq;
using Xunit;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Commands.Appointments
{
    public class CreateAppointmentCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ICurrentUserProvider> _currentUserProviderMock;
        private readonly Mock<ICheckAccessProvider> _checkAccessProviderMock;
        private static readonly Guid CurrentUserId = Guid.NewGuid();

        public CreateAppointmentCommandHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
            _currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(CurrentUserId);

            _checkAccessProviderMock = new Mock<ICheckAccessProvider>();
            _checkAccessProviderMock
                .Setup(_ => _.ShouldHaveSameHealthCenterAsync(It.IsAny<Guid[]>()));
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnNewItemIdAndAddCreatorId()
        {
            // Arrange
            var capturedServiceParams = new List<CreateAppointmentRequestDto>();

            var createdGuid = Guid.NewGuid();
            var appointmentCommandServiceApiMock = new Mock<IAppointmentCommandService>();
            appointmentCommandServiceApiMock
                .Setup(_ => _.CreateAppointment(Capture.In(capturedServiceParams), CancellationToken.None))
                .ReturnsAsync(createdGuid);

            var command = new CreateAppointmentCommand(
                "Test appointment",
                "Description",
                DateTime.Now.AddHours(1),
                TimeSpan.FromHours(1),
                AppointmentType.Urgent,
                new[] { Guid.NewGuid(), Guid.NewGuid() });
            var commandHandler = new CreateAppointmentCommandHandler(_mapper, _currentUserProviderMock.Object, _checkAccessProviderMock.Object, appointmentCommandServiceApiMock.Object);

            // Act
            var newItemId = await commandHandler.HandleAsync(command);

            // Assert
            Assert.Equal(createdGuid, newItemId);
            Assert.Single(capturedServiceParams);

            var serviceParams = capturedServiceParams.First();
            Assert.Equal(command.Title, serviceParams.Title);
            Assert.Equal(command.Description, serviceParams.Description);
            Assert.Equal(command.StartDate, serviceParams.StartDate);
            Assert.Equal(command.Duration, serviceParams.Duration);
            Assert.Equal((int)command.AppointmentType, (int)serviceParams.AppointmentType);
            Assert.Equal(CurrentUserId, serviceParams.CreatorId);
            Assert.Equal(CurrentUserId, serviceParams.CreatorId);
            Assert.Equal(command.AttendeeIds.Length, serviceParams.AttendeeIds!.Length);
        }
    }
}
