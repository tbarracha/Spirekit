using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Domain.Permissions.Models;
using SpireApi.Application.Modules.Iam.Domain.Roles.Models;
using SpireApi.Application.Modules.Iam.Domain.Users.Models;
using SpireCore.API.EntityFramework.DbContexts;

namespace SpireApi.Application.Modules.Iam.Infrastructure;

public abstract class BaseIamDbContext : BaseEntityDbContext
{
    protected BaseIamDbContext(DbContextOptions options) : base(options) { }

    // Users
    public DbSet<IamUser> IamUsers { get; set; } = default!;


    // Roles
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<RolePermission> RolePermissions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;


    // Permissions
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<PermissionScope> PermissionScopes { get; set; } = default!;


    // Groups
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupType> GroupTypes { get; set; }
    public DbSet<GroupMember> Memberships { get; set; }
    public DbSet<GroupMembershipState> MembershipsState { get; set; }
    public DbSet<GroupMemberAudit> MembershipsAudit { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Register all IEntityTypeConfiguration<T> implementations (if you have them)
        builder.ApplyConfigurationsFromAssembly(typeof(BaseIamDbContext).Assembly);

        // OPTIONAL: If you want to customize relationships further, do it here.
        // For example:
        // builder.Entity<GroupMember>()
        //     .HasOne(gm => gm.Role)
        //     .WithMany() // or .WithMany(r => r.GroupMembers) if you add navigation
        //     .HasForeignKey(gm => gm.RoleId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
