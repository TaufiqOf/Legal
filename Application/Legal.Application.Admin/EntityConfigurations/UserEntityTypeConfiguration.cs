using Legal.Application.Admin.Dtos;
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
