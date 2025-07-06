using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Domain.Models.Users;
using SpireApi.Shared.EntityFramework.ModelBuilders.Attributes.StoreAsStringAttribute;

namespace SpireApi.Application.Modules.Iam.Infrastructure;

public abstract class BaseIamDbContext : DbContext
{
    protected BaseIamDbContext(DbContextOptions options) : base(options) { }

    // Users
    public DbSet<IamUser> IamUsers { get; set; } = default!;


    // Groups
    public DbSet<Group> Groups { get; set; } = default!;
    public DbSet<GroupType> GroupTypes { get; set; } = default!;
    public DbSet<GroupMember> GroupMembers { get; set; } = default!;


    // Roles
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<RolePermission> RolePermissions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;


    // Permissions
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<PermissionScope> PermissionScopes { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Register all IEntityTypeConfiguration<T> implementations (if you have them)
        builder.ApplyConfigurationsFromAssembly(typeof(BaseIamDbContext).Assembly);

        builder.ApplyEnumStringConversions();

        // OPTIONAL: If you want to customize relationships further, do it here.
        // For example:
        // builder.Entity<GroupMember>()
        //     .HasOne(gm => gm.Role)
        //     .WithMany() // or .WithMany(r => r.GroupMembers) if you add navigation
        //     .HasForeignKey(gm => gm.RoleId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
