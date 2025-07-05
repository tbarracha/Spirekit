using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Shared.EntityFramework.Entities.Abstractions;
using SpireApi.Shared.JWT.Identity.Users;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Authentication.Domain.Models.AuthUserIdentities;

/// <summary>
/// Represents a User login identity (local or external provider) used for user authentication.
/// </summary>
public class AuthUserIdentity : IdentityUser<Guid>, IEntity<Guid>, IJwtUserIdentity
{
    // Identity provider info
    public string Provider { get; set; } = "local"; // e.g., "local", "google", "github"
    public string? ProviderUserId { get; set; } // For external identity mapping

    // Optional display and personal fields (for token use only)
    public string? DisplayName { get; set; } = string.Empty;
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    // Audit and account state
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

    public class Configuration : IEntityTypeConfiguration<AuthUserIdentity>
    {
        public void Configure(EntityTypeBuilder<AuthUserIdentity> builder)
        {
            builder.ToTable("AuthUserIdentities");

            builder.Property(i => i.Provider)
                   .IsRequired()
                   .HasMaxLength(32);

            builder.Property(i => i.ProviderUserId)
                   .HasMaxLength(128);

            builder.Property(i => i.FirstName)
                   .HasMaxLength(100);

            builder.Property(i => i.LastName)
                   .HasMaxLength(100);

            builder.Property(i => i.DisplayName)
                   .HasMaxLength(150);

            builder.Property(i => i.DateOfBirth);

            builder.Property(i => i.CreatedAt).IsRequired();
            builder.Property(i => i.UpdatedAt).IsRequired();
            builder.Property(i => i.StateFlag)
                   .IsRequired()
                   .HasMaxLength(1);

            builder.Property(i => i.LastLoginAt);
            builder.Property(i => i.LastLogoutAt);
            builder.Property(i => i.LastPasswordChangeAt);
            builder.Property(i => i.LastFailedLoginAt);

            builder.Property(i => i.LastLoginIp)
                   .HasMaxLength(64);

            builder.Property(i => i.LastLoginUserAgent)
                   .HasMaxLength(256);

            builder.Property(i => i.IsInitialPasswordChanged)
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}
