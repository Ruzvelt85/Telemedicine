using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Common.Infrastructure.Patterns.Data;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Telemedicine.Services.AppointmentDomainService.Core.Enums;
using Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers.Setup;
using Telemedicine.Services.AppointmentDomainService.WebAPI;
using Telemedicine.Services.AppointmentDomainService.WebAPI.Queries.WebClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using DomainCommon = Telemedicine.Services.AppointmentDomainService.API.Common.Common;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.QueryHandlers.WebClient
{
    public class NearestAppointmentsByAttendeeQueryHandlerTests : IDisposable
    {
        private readonly IReadRepository<Appointment> _readRepository;
        private readonly AppointmentTestDbContext _context;
        private readonly IMapper _mapper;

        public NearestAppointmentsByAttendeeQueryHandlerTests()
        {
            // For every unit test we have to specify different name of InMemory database
            // in order to prevent unstable results of tests due to only one service-provider
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentTestDbContext>().UseInMemoryDatabase($"WebClient_NearestAppointmentsByAttendeeQueryHandlerTests-{Guid.NewGuid()}");
            _context = new AppointmentTestDbContext(dbContextOptions.Options, GetMockOptions().Object, new NullLoggerFactory());
            _context.Database.EnsureCreated();
            _readRepository = new AppointmentTestReadRepository(_context);
            _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup).Assembly)).CreateMapper();
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendee_SuccessfulSimpleCase()
        {
            var (patientId, previousAppointmentId, nextAppointmentId) = GetNewGuidItems();

            var appointments = new[]
            {
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), patientId),
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddDays(1), patientId, AppointmentType.Urgent)
            };

            var response = await GetResponse(appointments, patientId);

            Assert.NotNull(response.PreviousAppointmentInfo);
            Assert.Equal(previousAppointmentId, response.PreviousAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentInfo);
            Assert.Equal(nextAppointmentId, response.NextAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentType);
            Assert.Equal(DomainCommon.AppointmentType.Urgent, response.NextAppointmentType!.Value);
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendeeWhenExistManyAppointmentsWithDifferentTypes()
        {
            var (patientId, previousAppointmentId, nextAppointmentId) = GetNewGuidItems();

            var appointments = new[]
            {
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-2), patientId, AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(1), patientId, AppointmentType.Urgent, AppointmentStatus.Opened),
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), patientId, AppointmentType.SemiAnnual, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddHours(-8), Guid.NewGuid(), AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddHours(-5), patientId, AppointmentType.Annual, AppointmentStatus.Opened, true),
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddHours(5), patientId, AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddHours(2), patientId, AppointmentType.Urgent, AppointmentStatus.Opened, true),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-3), patientId, AppointmentType.Urgent),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddHours(3), Guid.NewGuid(), AppointmentType.Annual),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(200), patientId)
            };

            var response = await GetResponse(appointments, patientId);

            Assert.NotNull(response.PreviousAppointmentInfo);
            Assert.Equal(previousAppointmentId, response.PreviousAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentInfo);
            Assert.Equal(nextAppointmentId, response.NextAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentType);
            Assert.Equal(DomainCommon.AppointmentType.FollowUp, response.NextAppointmentType!.Value);
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendeeWhenExistAppointmentsOnlyForAnotherAttendees()
        {
            var (patientId, previousAppointmentId, nextAppointmentId) = GetNewGuidItems();

            var appointments = new[]
            {
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-2), Guid.NewGuid(), AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(1), Guid.NewGuid(), AppointmentType.Urgent, AppointmentStatus.Opened),
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), Guid.NewGuid(), AppointmentType.SemiAnnual, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddHours(-5), Guid.NewGuid(), AppointmentType.Annual, AppointmentStatus.Opened, true),
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddHours(5), Guid.NewGuid(), AppointmentType.FollowUp, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddHours(2), Guid.NewGuid(), AppointmentType.Urgent, AppointmentStatus.Opened, true),
                GetAppointment(Guid.NewGuid(), "Previous Appointment", DateTime.UtcNow.AddDays(-3), Guid.NewGuid(), AppointmentType.Urgent, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddHours(10), Guid.NewGuid(), AppointmentType.Annual, AppointmentStatus.Opened),
                GetAppointment(Guid.NewGuid(), "Next Appointment", DateTime.UtcNow.AddDays(200), Guid.NewGuid())
            };

            var response = await GetResponse(appointments, patientId);

            Assert.Null(response.PreviousAppointmentInfo);
            Assert.Null(response.NextAppointmentInfo);
            Assert.Null(response.NextAppointmentType);
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendeeWhenNotExistPreviousAppointment()
        {
            var (patientId, _, nextAppointmentId) = GetNewGuidItems();

            var appointments = new[]
            {
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddDays(1), patientId, AppointmentType.Urgent)
            };

            var response = await GetResponse(appointments, patientId);

            Assert.Null(response.PreviousAppointmentInfo);
            Assert.NotNull(response.NextAppointmentInfo);
            Assert.Equal(nextAppointmentId, response.NextAppointmentInfo!.AppointmentId);
            Assert.NotNull(response.NextAppointmentType);
            Assert.Equal(DomainCommon.AppointmentType.Urgent, response.NextAppointmentType!.Value);
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendeeWhenNotExistNextAppointment()
        {
            var (patientId, previousAppointmentId, _) = GetNewGuidItems();

            var appointments = new[]
            {
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), patientId),
            };

            var response = await GetResponse(appointments, patientId);

            Assert.NotNull(response.PreviousAppointmentInfo);
            Assert.Equal(previousAppointmentId, response.PreviousAppointmentInfo!.AppointmentId);
            Assert.Null(response.NextAppointmentInfo);
            Assert.Null(response.NextAppointmentType);
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendeeWhenAppointmentsNotExist()
        {
            var patientId = Guid.NewGuid();

            var appointments = Array.Empty<Appointment>();

            var response = await GetResponse(appointments, patientId);

            Assert.Null(response.PreviousAppointmentInfo);
            Assert.Null(response.NextAppointmentInfo);
            Assert.Null(response.NextAppointmentType);
        }

        [Fact]
        public async Task FindNearestAppointmentsForAttendeeWhenExistOnlyDeletedAppointments()
        {
            var (patientId, previousAppointmentId, nextAppointmentId) = GetNewGuidItems();

            var appointments = new[]
            {
                GetAppointment(previousAppointmentId, "Previous Appointment", DateTime.UtcNow.AddDays(-1), patientId, AppointmentType.Urgent, AppointmentStatus.Opened, true),
                GetAppointment(nextAppointmentId, "Next Appointment", DateTime.UtcNow.AddDays(1), patientId, AppointmentType.Annual, AppointmentStatus.Opened, true)
            };

            var response = await GetResponse(appointments, patientId);

            Assert.Null(response.PreviousAppointmentInfo);
            Assert.Null(response.NextAppointmentInfo);
            Assert.Null(response.NextAppointmentType);
        }

        private static (Guid guid1, Guid guid2, Guid guid3) GetNewGuidItems()
        {
            return (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        }

        private Appointment GetAppointment(Guid appointmentId, string appointmentTitle,
            DateTime startDateTime, Guid patientId, AppointmentType type = AppointmentType.Default,
            AppointmentStatus status = AppointmentStatus.Default, bool isDeleted = false)
        {
            return new Fixture().Build<Appointment>()
                .With(e => e.Id, appointmentId)
                .With(e => e.Title, appointmentTitle)
                .With(e => e.StartDate, startDateTime)
                .With(e => e.Type, type)
                .With(e => e.Status, status)
                .With(e => e.IsDeleted, isDeleted)
                .With(e => e.Attendees, new List<AppointmentAttendee>()
                {
                    new Fixture().Build<AppointmentAttendee>()
                        .With(e => e.AppointmentId, appointmentId)
                        .With(e => e.UserId, patientId)
                        .With(e => e.IsDeleted, false)
                        .Create(),
                    new Fixture().Build<AppointmentAttendee>()
                        .With(e => e.AppointmentId, appointmentId)
                        .Create()
                })
                .Create();
        }

        private async Task<NearestAppointmentsResponseDto> GetResponse(Appointment[] appointments, Guid patientId)
        {
            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();

            var queryHandler = new NearestAppointmentsByAttendeeQueryHandler(_readRepository, _mapper);

            return await queryHandler.HandleAsync(new NearestAppointmentsByAttendeeQuery(patientId));
        }

        private Mock<IOptionsSnapshot<EfCoreDbContextSettings>> GetMockOptions()
        {
            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());

            return mockOptions;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
