using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL.CrossEntities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL.Configurations
{
    internal class InterdisciplinaryTeamConfiguration : EntityConfiguration<InterdisciplinaryTeam>
    {
        public override void Configure(EntityTypeBuilder<InterdisciplinaryTeam> builder)
        {
            base.Configure(builder);

            builder.ToTable("interdisciplinary_team");

            builder.Property(p => p.Name).IsRequired().HasMaxLength(FieldLengthConstants.InterdisciplinaryTeamLengths.NameLength);
            builder.Property(p => p.InnerId).IsRequired().HasMaxLength(FieldLengthConstants.InnerIdLength);

            builder.HasIndex(p => p.InnerId).IsUnique();

            builder
                .HasMany(p => p.Doctors)
                .WithMany(p => p.InterdisciplinaryTeams)
                .UsingEntity<InterdisciplinaryTeamDoctor>(
                    j => j
                        .HasOne(pt => pt.Doctor)
                        .WithMany()
                        .HasForeignKey(pt => pt.DoctorId),
                    j => j
                        .HasOne(pt => pt.InterdisciplinaryTeam)
                        .WithMany()
                        .HasForeignKey(pt => pt.InterdisciplinaryTeamId),
                    j =>
                    {
                        j.ToTable("interdisciplinary_team_doctor");
                        j.HasKey(t => new { t.InterdisciplinaryTeamId, t.DoctorId });
                    });
        }
    }
}
