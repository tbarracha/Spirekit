using Microsoft.EntityFrameworkCore;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions;
using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Infrastructure
{
    public abstract class BaseIamDbContext : GuidEntityDbContext
    {
        protected BaseIamDbContext(DbContextOptions options) : base(options) { }

        // DbSets for your IAM domain
        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<GroupType> GroupTypes { get; set; } = default!;
        public DbSet<GroupMember> GroupMembers { get; set; } = default!;

        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<RolePermission> RolePermissions { get; set; } = default!;

        public DbSet<Permission> Permissions { get; set; } = default!;
        public DbSet<PermissionScope> PermissionScopes { get; set; } = default!;

        // If you have User entity or similar, add here

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
}
