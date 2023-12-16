using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;

namespace Telemedicine.Services.HealthMeasurementDomainService.DAL.Configurations
{
    public class PulseRateMeasurementConfiguration : EntityConfiguration<PulseRateMeasurement>
    {
        public override void Configure(EntityTypeBuilder<PulseRateMeasurement> builder)
        {
            base.Configure(builder);

            builder.ToTable("pulse_rate_measurement");
            builder.Property(e => e.PatientId).IsRequired();
            builder.Property(e => e.PulseRate).IsRequired();
            builder.Property(e => e.ClientDate).IsRequired();
            builder.HasIndex(e => new { e.PatientId, e.ClientDate });
        }
    }
}