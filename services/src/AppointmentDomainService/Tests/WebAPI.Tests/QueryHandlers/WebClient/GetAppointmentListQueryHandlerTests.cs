using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.WebAPI;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.WebClient;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers.WebClient
{
    public class GetAppointmentListQueryHandlerTests
    {
        private readonly AppointmentDomainServiceDbContext _context;
        private readonly GetAppointmentListQueryHandler _queryHandler;

        public GetAppointmentListQueryHandlerTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase($"WebClient_GetAppointmentListQueryHandlerTests-{Guid.NewGuid()}");

            _context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();

            IMapper mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();

            _queryHandler = new GetAppointmentListQueryHandler(mapper, new AppointmentReadRepository(_context));
        }

        [Fact]
        public async Task GetAppointmentList_CommonSuccessfulTest()
        {
            // Arrange
            var (doctorId, firstAppointmentId, lastAppointmentId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var appointments = new List<Appointment>()
            {
                GetAppointment(Guid.NewGuid(), "Found Appointment", new DateTime(2021, 11, 15), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Found Appointment", new DateTime(2021, 11, 14), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(lastAppointmentId, "Found Appointment", new DateTime(2021, 11, 13), doctorId, status: AppointmentStatus.Opened),
                GetAppointment(firstAppointmentId, "Found Appointment", new DateTime(2021, 11, 16), doctorId, status: AppointmentStatus.Opened)
            };

            await _context.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetAppointmentListQuery(new AppointmentListFilterRequestDto
            {
                DateRange = new Range<DateTime?>(new DateTime(2021, 11, 10), new DateTime(2021, 11, 20)),
                AttendeeId = doctorId,
                AppointmentStates = new[] { API.Common.Common.AppointmentState.Done }
            },
                new PagingRequestDto());

            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(appointments.Count, result.Items.Count);
            Assert.Equal(firstAppointmentId, result.Items.First().Id);
            Assert.Equal(lastAppointmentId, result.Items.Last().Id);
        }

        [Fact]
        public async Task GetAppointmentList_NotFoundForSpecifiedDoctor_ShouldNotReturnAppointment()
        {
            // Arrange
            var (doctorId, appointmentId) = (Guid.NewGuid(), Guid.NewGuid());
            var attendees = new List<AppointmentAttendee>()
            {
                new Fixture().Build<AppointmentAttendee>()
                    .With(_ => _.AppointmentId, appointmentId)
                    .With(_ => _.IsDeleted, false)
                    .Create(),
                new Fixture().Build<AppointmentAttendee>()
                    .With(_ => _.AppointmentId, appointmentId)
                    .With(_ => _.IsDeleted, false)
                    .Create()
            };

            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.IsDeleted, false)
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetAppointmentListQuery(
                new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(null, null),
                    AttendeeId = doctorId
                },
                new PagingRequestDto());

            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAppointmentList_DeletedAppointment_ShouldNotReturnAppointment()
        {
            // Arrange
            var (doctorId, appointmentId) = (Guid.NewGuid(), Guid.NewGuid());
            var attendees = new List<AppointmentAttendee>()
            {
                new Fixture().Build<AppointmentAttendee>()
                    .With(_ => _.AppointmentId, appointmentId)
                    .With(_ => _.UserId, doctorId)
                    .With(_ => _.IsDeleted, false)
                    .Create(),
                new Fixture().Build<AppointmentAttendee>()
                    .With(_ => _.AppointmentId, appointmentId)
                    .With(_ => _.IsDeleted, false)
                    .Create()
            };

            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, new DateTime(2021, 11, 15))
                .With(_ => _.IsDeleted, true)
                .With(_ => _.Attendees, attendees).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetAppointmentListQuery(
                new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(new DateTime(2021, 11, 10), new DateTime(2021, 11, 20)),
                    AttendeeId = doctorId
                },
                new PagingRequestDto());

            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAppointmentList_NoAttendees_ShouldNotReturnAppointment()
        {
            // Arrange
            var (doctorId, appointmentId) = (Guid.NewGuid(), Guid.NewGuid());

            var appointment = new Fixture().Build<Appointment>()
                .With(_ => _.Id, appointmentId)
                .With(_ => _.StartDate, new DateTime(2021, 11, 15))
                .With(_ => _.IsDeleted, false).Create();
            await _context.AddRangeAsync(appointment);
            await _context.SaveChangesAsync();

            // Act
            var query = new GetAppointmentListQuery(
                new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(new DateTime(2021, 11, 10), new DateTime(2021, 11, 20)),
                    AttendeeId = doctorId
                },
                new PagingRequestDto());

            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAppointmentList_EmptyRepository_ShouldReturnEmptyAppointmentList()
        {
            // Act
            var query = new GetAppointmentListQuery(
                new AppointmentListFilterRequestDto
                {
                    DateRange = new Range<DateTime?>(null, null),
                    AttendeeId = Guid.NewGuid()
                },
                new PagingRequestDto());

            var result = await _queryHandler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        private static Appointment GetAppointment(Guid appointmentId, string appointmentName,
            DateTime startDateTime, Guid attendeeId, AppointmentType type = AppointmentType.Default,
            AppointmentStatus status = AppointmentStatus.Default, bool isDeleted = false)
        {
            return new Fixture().Build<Appointment>()
                .With(e => e.Id, appointmentId)
                .With(e => e.Title, appointmentName)
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Type, type)
                .With(e => e.Status, status)
                .With(e => e.IsDeleted, isDeleted)
                .With(e => e.Attendees, new List<AppointmentAttendee>()
                {
                    new Fixture().Build<AppointmentAttendee>()
                        .With(e => e.AppointmentId, appointmentId)
                        .With(e => e.UserId, attendeeId)
                        .With(e => e.IsDeleted, false)
                        .Create()
                })
                .Create();
        }
    }
}
