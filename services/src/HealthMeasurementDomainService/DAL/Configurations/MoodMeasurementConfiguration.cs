﻿using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Services.HealthMeasurementDomainService.DAL.Configurations
{
    public class MoodMeasurementConfiguration : EntityConfiguration<MoodMeasurement>
    {
        public override void Configure(EntityTypeBuilder<MoodMeasurement> builder)
        {
            base.Configure(builder);

            builder.ToTable("mood_measurement");
            builder.Property(e => e.PatientId).IsRequired();
            builder.Property(e => e.Measure).IsRequired();
            builder.Property(e => e.ClientDate).IsRequired();
            builder.HasIndex(e => new { e.PatientId, e.ClientDate });
        }
    }
}
