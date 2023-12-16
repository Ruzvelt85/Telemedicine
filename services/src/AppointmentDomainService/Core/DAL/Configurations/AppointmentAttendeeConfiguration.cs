using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Services.AppointmentDomainService.Core.DAL.Configurations
{
    public class AppointmentAttendeeConfiguration : EntityConfiguration<AppointmentAttendee>
    {
        public override void Configure(EntityTypeBuilder<AppointmentAttendee> builder)
        {
            base.Configure(builder);

            builder.ToTable("appointment_attendee");

            builder.Property(e => e.AppointmentId).IsRequired();
            builder.Property(e => e.UserId).IsRequired();
        }
    }
}
