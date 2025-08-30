using Legal.Application.Admin.Dtos;
using Legal.Service.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Application.Admin;

/// <summary>
///     Database context for the expense application
/// </summary>
public class AdminDatabaseContext : ADatabaseContext
{
    public AdminDatabaseContext()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdminDatabaseContext" /> class.
    ///     Database context for the expense application
    /// </summary>
    /// <remarks>
    ///     This context is used by the expense application. It is configured to use
    ///     the <c>expense</c> database.
    /// </remarks>
    public AdminDatabaseContext(DbContextOptions<AdminDatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<User> Owners { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(ModuleName.ADMIN.ToString().ToLower());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminDatabaseContext).Assembly);
    }

    public override string SchemaName()
    {
        return ModuleName.ADMIN.ToString().ToLower();
    }
}