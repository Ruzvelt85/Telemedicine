using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Services.HealthMeasurementDomainService.DAL.Configurations
{
    public class BloodPressureMeasurementConfiguration : EntityConfiguration<BloodPressureMeasurement>
    {
        public override void Configure(EntityTypeBuilder<BloodPressureMeasurement> builder)
        {
            base.Configure(builder);

            builder.ToTable("blood_pressure_measurement");
            builder.Property(e => e.PatientId).IsRequired();
            builder.Property(e => e.Systolic).IsRequired();
            builder.Property(e => e.Diastolic).IsRequired();
            builder.Property(e => e.PulseRate).IsRequired();
            builder.Property(e => e.ClientDate).IsRequired();

            builder.HasIndex(e => new { e.PatientId, e.ClientDate });
        }
    }
}
