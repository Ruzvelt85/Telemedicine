using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.Constants;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;

namespace Telemedicine.Services.AppointmentDomainService.Core.DAL.Configurations
{
    public class AppointmentConfiguration : EntityConfiguration<Appointment>
    {
        public override void Configure(EntityTypeBuilder<Appointment> builder)
        {
            base.Configure(builder);

            builder.ToTable("appointment");

            builder.Property(e => e.Title).IsRequired().HasMaxLength(FieldLengthConstants.Title);
            builder.Property(e => e.StartDate).IsRequired();
            builder.Property(e => e.Duration).IsRequired();
            builder.Property(e => e.Type).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.CancelReason).HasMaxLength(FieldLengthConstants.CancelReason);
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
