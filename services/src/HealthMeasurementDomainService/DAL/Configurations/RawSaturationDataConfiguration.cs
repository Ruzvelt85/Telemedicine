using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;

namespace Telemedicine.Services.HealthMeasurementDomainService.DAL.Configurations
{
    public class RawSaturationDataConfiguration : IEntityTypeConfiguration<RawSaturationData>
    {
        public void Configure(EntityTypeBuilder<RawSaturationData> builder)
        {
            builder.ToTable("saturation_measurement_raw");

            builder.Property(e => e.Items)
                .HasColumnType("jsonb")
                .IsRequired();
        }
    }
}
