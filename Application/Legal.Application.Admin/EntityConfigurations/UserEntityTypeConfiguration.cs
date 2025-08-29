using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legal.Application.Admin.EntityConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("user", "public")
            .Metadata
            .SetIsTableExcludedFromMigrations(false);
    }
}

public class ContractEntityTypeConfiguration : AuditableEntityTypeConfiguration<Models.Contract>
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<Models.Contract> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Name).IsRequired();

        builder.Property(e => e.Author).IsRequired();

        builder.Property(e => e.Description);
    }
}