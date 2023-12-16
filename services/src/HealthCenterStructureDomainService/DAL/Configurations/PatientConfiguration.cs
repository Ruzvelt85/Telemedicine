using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL.Configurations
{
    internal class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("patient");

            builder.Property(p => p.HealthCenterId).IsRequired();

            builder.HasOne(p => p.InterdisciplinaryTeam)
                .WithMany()
                .HasForeignKey(p => p.InterdisciplinaryTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.HealthCenter)
                .WithMany()
                .HasForeignKey(p => p.HealthCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.PrimaryCareProvider)
                .WithMany()
                .HasForeignKey(p => p.PrimaryCareProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
