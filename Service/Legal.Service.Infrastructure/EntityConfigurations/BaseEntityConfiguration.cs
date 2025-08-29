using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Legal.Service.Infrastructure.EntityConfigurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : DomainBaseModel
{
    public virtual void Configure([NotNull] EntityTypeBuilder<TEntity> builder)
    {
        builder
            .Property(e => e.Id)
            .HasColumnType("varchar(36)")
            .HasMaxLength(36);

        builder
            .Property(x => x.CreatedBy)
            .HasColumnType("varchar(36)")
            .HasMaxLength(36);

        builder
            .Property(x => x.LastModifiedBy)
            .HasColumnType("varchar(36)")
            .HasMaxLength(36);

        builder
            .Property(x => x.CreateTime)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("now()")
            .HasColumnType("timestamptz");

        builder
            .Property(x => x.LastModifiedTime)
            .HasColumnType("timestamptz")
            .ValueGeneratedNever();

        builder
            .Property(e => e.IsDeleted)
            .HasDefaultValue(false);

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