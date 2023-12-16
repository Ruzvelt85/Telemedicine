using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL.CrossEntities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL.Configurations
{
    internal class HealthCenterConfiguration : EntityConfiguration<HealthCenter>
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<HealthCenter> builder)
        {
            base.Configure(builder);

            builder.ToTable("health_center");

            builder.Property(p => p.Name).IsRequired().HasMaxLength(FieldLengthConstants.HealthCenterLengths.NameLength);
            builder.Property(p => p.InnerId).IsRequired().HasMaxLength(FieldLengthConstants.InnerIdLength);
            builder.Property(p => p.UsaState).IsRequired().HasMaxLength(FieldLengthConstants.HealthCenterLengths.UsaStateLength);

            builder.HasIndex(p => p.InnerId).IsUnique();

            builder.HasMany<InterdisciplinaryTeam>()
                .WithOne()
                .HasForeignKey(idt => idt.HealthCenterId);

            builder
                .HasMany(pc => pc.Doctors)
                .WithMany(me => me.HealthCenters)
                .UsingEntity<HealthCenterDoctor>(
                    j => j
                        .HasOne(_ => _.Doctor)
                        .WithMany()
                        .HasForeignKey(_ => _.DoctorId),
                    j => j
                        .HasOne(_ => _.HealthCenter)
                        .WithMany()
                        .HasForeignKey(_ => _.HealthCenterId),
                    j =>
                    {
                        j.ToTable("health_center_doctor");
                        j.HasKey(_ => new { _.HealthCenterId, _.DoctorId });
                    });
        }
    }
}
