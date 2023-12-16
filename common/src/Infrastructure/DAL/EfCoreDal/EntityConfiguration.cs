using Telemedicine.Common.Infrastructure.EntityBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Telemedicine.Common.Infrastructure.DAL.EfCoreDal
{
    public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);

            if (typeof(TEntity).IsAssignableTo(typeof(IOptimisticLock)))
            {
                builder.Property(nameof(IOptimisticLock.Timestamp)) //for Postgres, there is no default optimistic locking mechanism, so this is how it's done instead, through system column 'xmin'. (taken from here https://github.com/npgsql/efcore.pg/issues/19)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
            }

            if (typeof(TEntity).IsAssignableTo(typeof(IAuditable)))
            {
                builder.Property(nameof(IAuditable.CreatedOn)).IsRequired();
                builder.Property(nameof(IAuditable.UpdatedOn)).IsRequired();
            }

            if (typeof(TEntity).IsAssignableTo(typeof(ILogicallyDeletable)))
            {
                builder.Property(nameof(ILogicallyDeletable.IsDeleted)).IsRequired().HasDefaultValue(false);
                builder.HasQueryFilter(e => !((ILogicallyDeletable)e).IsDeleted);
            }

            if (typeof(TEntity).IsAssignableTo(typeof(IHasDomainEvents)))
            {
                builder.Ignore(nameof(IHasDomainEvents.DomainEvents));
            }
        }
    }
}
