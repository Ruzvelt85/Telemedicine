using AutoMapper;
using System;
using System.Linq;
using Xunit;
using Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using Telemedicine.Services.WebClientBffService.API.Appointments.AppointmentQueryService.Dto.GetAppointmentList;
using Telemedicine.Services.WebClientBffService.WebAPI.Mappings;
using AppointmentByIdResponseDto = Telemedicine.Services.AppointmentDomainService.API.Common.AppointmentQueryService.Dto.AppointmentByIdResponseDto;
using BffCommon = Telemedicine.Services.WebClientBffService.API.Common;
using DomainServiceCommon = Telemedicine.Services.AppointmentDomainService.API.Common;

namespace Telemedicine.Services.WebClientBffService.Tests.WebAPI.Tests.Mappers
{
    public class AppointmentProfileTests
    {
        private readonly IMapper _mapper;
        private readonly AppointmentByIdResponseDto _model;

        public AppointmentProfileTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(AppointmentProfile).Assembly)).CreateMapper();
            _model = new AppointmentByIdResponseDto(Guid.NewGuid(), "Title", "Description",
                DateTime.UtcNow, TimeSpan.FromMinutes(45), AppointmentType.FollowUp, AppointmentState.Default, 192);
        }

        [Theory]
        [InlineData(AppointmentState.Opened, BffCommon.AppointmentStatus.Opened)]
        [InlineData(AppointmentState.Ongoing, BffCommon.AppointmentStatus.Opened)]
        [InlineData(AppointmentState.Cancelled, BffCommon.AppointmentStatus.Cancelled)]
        [InlineData(AppointmentState.Missed, BffCommon.AppointmentStatus.Cancelled)]
        [InlineData(AppointmentState.Done, BffCommon.AppointmentStatus.Done)]
        public void MappingFromAppointmentStateToAppointmentStatus_ShouldBeCorrect2(AppointmentState state, BffCommon.AppointmentStatus expectedStatus)
        {
            var sourceModel = _model with { State = state };

            var actualModel = _mapper.Map<API.Appointments.AppointmentQueryService.Dto.GetAppointmentById.AppointmentByIdResponseDto>(sourceModel);

            Assert.NotNull(actualModel);
            Assert.Equal(sourceModel.Id, actualModel.Id);
            Assert.Equal(sourceModel.Title, actualModel.Title);
            Assert.Equal(sourceModel.StartDate, actualModel.StartDate);
            Assert.Equal(sourceModel.Duration, actualModel.Duration);
            Assert.Equal(sourceModel.Type.ToString(), actualModel.Type.ToString());
            Assert.Equal(sourceModel.Attendees.Count, actualModel.Attendees.Count);
            Assert.Equal(sourceModel.Rating, actualModel.Rating);
            Assert.Equal(expectedStatus, actualModel.Status);
        }

        [Theory]
        [InlineData(AppointmentState.Opened, BffCommon.AppointmentStatus.Opened)]
        [InlineData(AppointmentState.Ongoing, BffCommon.AppointmentStatus.Opened)]
        [InlineData(AppointmentState.Cancelled, BffCommon.AppointmentStatus.Cancelled)]
        [InlineData(AppointmentState.Missed, BffCommon.AppointmentStatus.Cancelled)]
        [InlineData(AppointmentState.Done, BffCommon.AppointmentStatus.Done)]
        public void MappingFromAppointmentStateToAppointmentStatus_ShouldBeCorrect(AppointmentState state, BffCommon.AppointmentStatus expectedStatus)
        {
            var domainDto = new DomainServiceCommon.WebClientBFFQueryService.Dto.AppointmentResponseDto
            {
                State = state
            };

            var bffDto = _mapper.Map<AppointmentResponseDto>(domainDto);

            Assert.NotNull(bffDto);
            Assert.Equal(expectedStatus, bffDto.Status);
        }

        [Theory]
        [InlineData(BffCommon.AppointmentStatus.All, new[] { AppointmentState.All })]
        [InlineData(BffCommon.AppointmentStatus.Opened, new[] { AppointmentState.Opened, AppointmentState.Ongoing })]
        [InlineData(BffCommon.AppointmentStatus.Cancelled, new[] { AppointmentState.Cancelled, AppointmentState.Missed })]
        [InlineData(BffCommon.AppointmentStatus.Done, new[] { AppointmentState.Done })]
        [InlineData((BffCommon.AppointmentStatus)int.MaxValue, new[] { AppointmentState.Default })]
        public void MappingFromAppointmentStatusToAppointmentState_ShouldBeCorrect(BffCommon.AppointmentStatus status, AppointmentState[] expectedState)
        {
            var bffFilterRequestDto = new AppointmentListFilterRequestDto
            {
                AppointmentStatus = status
            };

            var serviceFilterRequestDto = _mapper.Map<DomainServiceCommon.WebClientBFFQueryService.Dto.AppointmentListFilterRequestDto>(bffFilterRequestDto);

            Assert.NotNull(serviceFilterRequestDto);
            Assert.Equal(expectedState.Length, serviceFilterRequestDto.AppointmentStates!.Count);
            Assert.Empty(expectedState.Except(serviceFilterRequestDto.AppointmentStates));
        }
    }
}
