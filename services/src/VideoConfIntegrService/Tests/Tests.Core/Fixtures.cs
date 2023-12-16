using AutoFixture;
using System;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;

namespace Telemedicine.Services.VideoConfIntegrService.Tests.Tests.Core
{
    public static class Fixtures
    {
        public static Conference CreateConferenceFixture(Guid appointmentId, string appointmentTitle, int roomId, string roomUrl, string roomPin/*, string? xmlResponse = null*/)
        {
            return new Fixture()
                .Build<Conference>()
                .With(e => e.AppointmentId, appointmentId)
                .With(e => e.AppointmentTitle, appointmentTitle)
                .With(e => e.RoomId, roomId)
                .With(e => e.RoomUrl, roomUrl)
                .With(e => e.RoomPin, roomPin)
                //.With(e => e.XmlResponse, xmlResponse)
                .With(e => e.IsDeleted, false)
                .Create();
        }
    }
}
