using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService;
using Telemedicine.Services.AppointmentDomainService.API.Common.WebClientBFFQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients;
using Moq;
using Xunit;
using PatientListFilterRequestDto = Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList.PatientListFilterRequestDto;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Queries.Patients
{
    public class GetPatientListQueryHandlerTests
    {
        [Fact]
        public async Task HandleAsync_GetPatientList_OrderShouldBeUnchanged()
        {
            //Arrange
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            // Domain service is mocked, user id is not used
            currentUserProviderMock
                .Setup(m => m.GetId())
                .Returns(Guid.NewGuid());

            var patientListResponseDto = GetPatientListResponseDto();
            var healthCenterStructureServiceApiMock = new Mock<IPatientsQueryService>();
            healthCenterStructureServiceApiMock
                .Setup(m => m.GetPatientListAsync(It.IsAny<PatientListRequestDto>(), CancellationToken.None))
                .ReturnsAsync(patientListResponseDto);

            var appointmentServiceApiMock = new Mock<IWebClientBffQueryService>();
            appointmentServiceApiMock
                .Setup(m => m.GetNearestAppointmentsByAttendeeIdAsync(It.IsAny<Guid>(), CancellationToken.None))
                .Returns(async () =>
                {
                    await Task.Yield();
                    return new NearestAppointmentsResponseDto();
                });

            var mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(WebClientBffService.WebAPI.Startup).Assembly)).CreateMapper();

            var queryHandler = new GetPatientListQueryHandler(mapper, currentUserProviderMock.Object,
                healthCenterStructureServiceApiMock.Object, appointmentServiceApiMock.Object);

            var query = new GetPatientListQuery(
                new PatientListFilterRequestDto(),
                new PagingRequestDto(),
                SortingType.Asc
            );

            //Act
            var response = await queryHandler.HandleAsync(query);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.Equal(5, response.Items!.Count);

            var expectedItems = patientListResponseDto.Items.ToList();
            var actualItems = response.Items.ToList();

            for (var i = 0; i < actualItems.Count; i++)
            {
                Assert.Equal(expectedItems[i].Id, actualItems[i].Id);
                Assert.Equal(expectedItems[i].FirstName, actualItems[i].FirstName);
                Assert.Equal(expectedItems[i].LastName, actualItems[i].LastName);
                Assert.Equal(expectedItems[i].PhoneNumber, actualItems[i].PhoneNumber);
            }
        }

        private static PatientListResponseDto GetPatientListResponseDto()
        {
            return new PatientListResponseDto()
            {
                Items = new List<PatientResponseDto>
                {
                    new PatientResponseDto(Guid.NewGuid(), "12343a5546", "12343a5546", "Abce", "12345678"),
                    new PatientResponseDto(Guid.NewGuid(), "123456789", "123456789", "Abcd", "87654321"),
                    new PatientResponseDto(Guid.NewGuid(), "1234da25631", "1234da25631", "Abch", "11223344"),
                    new PatientResponseDto(Guid.NewGuid(), "1231dd23563", "1231dd23563", "Abcf", "44332211"),
                    new PatientResponseDto(Guid.NewGuid(), "123425631", "123425631", "Udsfdf", "11122233")
                }
            };
        }
    }
}
