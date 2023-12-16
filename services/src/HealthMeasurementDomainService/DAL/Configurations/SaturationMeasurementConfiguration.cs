using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;

namespace Telemedicine.Services.HealthMeasurementDomainService.DAL.Configurations
{
    public class SaturationMeasurementConfiguration : EntityConfiguration<SaturationMeasurement>
    {
        public override void Configure(EntityTypeBuilder<SaturationMeasurement> builder)
        {
            base.Configure(builder);

            builder.ToTable("saturation_measurement");
            builder.Property(e => e.PatientId).IsRequired();
            builder.Property(e => e.ClientDate).IsRequired();
            builder.Property(e => e.SpO2).IsRequired();
            builder.Property(e => e.PulseRate).IsRequired();
            builder.Property(e => e.Pi).IsRequired();
            builder.HasIndex(e => new { e.PatientId, e.ClientDate });

            builder.HasOne(e => e.RawSaturationData)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey<RawSaturationData>(el => el.Id);
        }
    }
}
