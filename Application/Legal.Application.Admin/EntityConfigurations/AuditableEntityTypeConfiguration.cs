using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legal.Application.Admin.EntityConfigurations;

public class AuditableEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : DomainBaseModel
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.LastModifiedByUser)
            .WithMany()
            .HasForeignKey(e => e.LastModifiedBy)
            .OnDelete(DeleteBehavior.SetNull);
    }
}