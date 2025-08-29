using Legal.Application.Admin.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legal.Application.Admin.EntityConfigurations;

public class ContractEntityTypeConfiguration : AuditableEntityTypeConfiguration<Contract>
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<Contract> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Name).IsRequired();

        builder.Property(e => e.Author).IsRequired();

        builder.Property(e => e.Description);
    }
}