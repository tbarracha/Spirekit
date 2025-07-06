using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireCore.API.EntityFramework.Entities.Base;
using SpireCore.API.JWT.Identity.Users;
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

    void IEntity<Guid>.ConfigureEntity<T>(EntityTypeBuilder<T> builder)
    {
        // Ensure this is only applied for the right entity type.
        if (builder is not EntityTypeBuilder<AuthUserIdentity> b)
            return;

        b.ToTable("AuthUserIdentities");

        b.Property(i => i.Provider)
            .IsRequired()
            .HasMaxLength(32);

        b.Property(i => i.ProviderUserId)
            .HasMaxLength(128);

        b.Property(i => i.FirstName)
            .HasMaxLength(100);

        b.Property(i => i.LastName)
            .HasMaxLength(100);

        b.Property(i => i.DisplayName)
            .HasMaxLength(150);

        b.Property(i => i.DateOfBirth);

        b.Property(i => i.CreatedAt).IsRequired();
        b.Property(i => i.UpdatedAt).IsRequired();
        b.Property(i => i.StateFlag)
            .IsRequired()
            .HasMaxLength(1);

        b.Property(i => i.LastLoginAt);
        b.Property(i => i.LastLogoutAt);
        b.Property(i => i.LastPasswordChangeAt);
        b.Property(i => i.LastFailedLoginAt);

        b.Property(i => i.LastLoginIp)
            .HasMaxLength(64);

        b.Property(i => i.LastLoginUserAgent)
            .HasMaxLength(256);

        b.Property(i => i.IsInitialPasswordChanged)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
