using AutoMapper;
using System;
using Xunit;
using Telemedicine.Services.MobileClientBffService.API;
using Telemedicine.Services.MobileClientBffService.API.MobileClientQueryService.Dto;
using Telemedicine.Services.MobileClientBffService.WebAPI.Mappers;
using DomainCommon = Telemedicine.Services.AppointmentDomainService.API.Common.Common;
using DomainService = Telemedicine.Services.AppointmentDomainService.API.Common.MobileClientBffQueryService.Dto;

namespace Telemedicine.Services.MobileClientBffService.Tests.WebAPI.Tests.Mappers
{
    public class LastChangedDataProfileTests
    {
        private readonly IMapper _mapper;
        private readonly DomainService.AppointmentResponseDto _model;

        public LastChangedDataProfileTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddMaps(typeof(LastChangedDataProfile).Assembly)).CreateMapper();
            _model = new DomainService.AppointmentResponseDto(Guid.NewGuid(), "TestTitle", DateTime.UtcNow, TimeSpan.FromMinutes(45),
                DomainCommon.AppointmentState.Default, DomainCommon.AppointmentType.Annual, DateTime.UtcNow, false);
        }

        [Theory]
        [InlineData(DomainCommon.AppointmentState.Default, AppointmentStatus.Default)]
        [InlineData(DomainCommon.AppointmentState.Opened, AppointmentStatus.Opened)]
        [InlineData(DomainCommon.AppointmentState.Ongoing, AppointmentStatus.Opened)]
        [InlineData(DomainCommon.AppointmentState.Cancelled, AppointmentStatus.Cancelled)]
        [InlineData(DomainCommon.AppointmentState.Missed, AppointmentStatus.Cancelled)]
        [InlineData(DomainCommon.AppointmentState.Done, AppointmentStatus.Done)]
        public void MappingFromAppointmentStateToAppointmentStatus_ShouldBeCorrect(DomainCommon.AppointmentState state, AppointmentStatus expectedStatus)
        {
            var sourceModel = _model with { State = state };

            var actualModel = _mapper.Map<AppointmentResponseDto>(sourceModel);

            Assert.NotNull(actualModel);
            Assert.Equal(sourceModel.Id, actualModel.Id);
            Assert.Equal(sourceModel.Title, actualModel.Title);
            Assert.Equal(sourceModel.StartDate, actualModel.StartDate);
            Assert.Equal(sourceModel.Duration, actualModel.Duration);
            Assert.Equal(sourceModel.Type.ToString(), actualModel.Type.ToString());
            Assert.Equal(sourceModel.UpdatedOn, actualModel.UpdatedOn);
            Assert.Equal(sourceModel.IsDeleted, actualModel.IsDeleted);
            Assert.Equal(sourceModel.Attendees.Count, actualModel.Attendees.Count);
            Assert.Equal(expectedStatus, actualModel.Status);
        }
    }
}
