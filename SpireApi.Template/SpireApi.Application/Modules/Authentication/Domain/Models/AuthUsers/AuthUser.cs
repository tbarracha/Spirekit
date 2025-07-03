using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Shared.JWT.UserIdentity;
using SpireCore.Abstractions.Interfaces;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;

public class AuthUser : IdentityUser<Guid>, ICreatedAt, IUpdatedAt, IStateFlag, IJwtUser
{
    // Official/internal use
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? DisplayName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }


    // Auditing and account state
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string StateFlag { get; set; } = StateFlags.ACTIVE;

    public DateTime? LastLoginAt { get; set; }
    public DateTime? LastLogoutAt { get; set; }
    public DateTime? LastPasswordChangeAt { get; set; }
    public DateTime? LastFailedLoginAt { get; set; }

    public string? LastLoginIp { get; set; }
    public string? LastLoginUserAgent { get; set; }

    public bool IsInitialPasswordChanged { get; set; } = false;

    public class Configuration : IEntityTypeConfiguration<AuthUser>
    {
        public void Configure(EntityTypeBuilder<AuthUser> builder)
        {
            builder.ToTable("AuthUsers");

            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.DateOfBirth);

            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt).IsRequired();
            builder.Property(u => u.StateFlag)
                   .IsRequired()
                   .HasMaxLength(1);

            builder.Property(u => u.LastLoginAt);
            builder.Property(u => u.LastLogoutAt);
            builder.Property(u => u.LastPasswordChangeAt);
            builder.Property(u => u.LastFailedLoginAt);

            builder.Property(u => u.LastLoginIp)
                   .HasMaxLength(64);

            builder.Property(u => u.LastLoginUserAgent)
                   .HasMaxLength(256);

            builder.Property(u => u.IsInitialPasswordChanged)
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}
