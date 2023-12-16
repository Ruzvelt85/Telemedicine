using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Services.HealthCenterStructureDomainService.DAL.Configurations
{
    internal class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("user");

            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(FieldLengthConstants.FirstOrLastNameLength);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(FieldLengthConstants.FirstOrLastNameLength);
            builder.Property(p => p.InnerId).IsRequired().HasMaxLength(FieldLengthConstants.InnerIdLength);
            builder.Property(p => p.Type).IsRequired();

            builder.HasIndex(p => p.FirstName);
            builder.HasIndex(p => p.LastName);
            builder.HasIndex(p => new { p.InnerId, p.Type }).IsUnique();
        }
    }
}
