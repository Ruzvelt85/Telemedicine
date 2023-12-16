using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.VideoConfIntegrService.Core.Constants;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Services.VideoConfIntegrService.DAL.Configurations
{
    public class ConferenceConfiguration : EntityConfiguration<Conference>
    {
        public override void Configure(EntityTypeBuilder<Conference> builder)
        {
            base.Configure(builder);

            builder.ToTable("conference");
            builder.Property(e => e.AppointmentId).IsRequired();
            builder.Property(e => e.AppointmentTitle).IsRequired().HasMaxLength(FieldLengthConstants.AppointmentTitle);
            builder.Property(e => e.AppointmentStartDate).IsRequired();
            builder.Property(e => e.AppointmentDuration).IsRequired();
            builder.Property(e => e.FullExtension).IsRequired().HasMaxLength(FieldLengthConstants.FullExtension);
            builder.Property(e => e.RoomId).IsRequired();
            builder.Property(e => e.RoomUrl).IsRequired().HasMaxLength(FieldLengthConstants.Url);
            builder.Property(e => e.RoomPin).HasMaxLength(FieldLengthConstants.PinCode);
            builder.Property(e => e.XmlResponse).IsRequired();

            builder.HasIndex(r => r.AppointmentId).IsUnique().HasFilter("is_deleted = false");
        }
    }
}
