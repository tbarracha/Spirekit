using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Application.Modules.Authentication.Domain.Models.AuthUsers;
using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Authentication.Domain.Models.AuthAudits;

public class AuthAudit : GuidEntity
{
    public Guid AuthUserId { get; set; }
    public AuthUser AuthUser { get; set; } = default!;

    public string Type { get; set; } = string.Empty; // AuthAuditType.Login, etc.

    public bool WasSuccessful { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string? FailureReason { get; set; }

    /// <summary>
    /// Override and extend the entity configuration for AuthAudit.
    /// </summary>
    public override void ConfigureEntity<T>(EntityTypeBuilder<T> builder)
    {
        // Call base with generic type
        base.ConfigureEntity(builder);

        // Now you can safely cast builder to EntityTypeBuilder<AuthAudit> if needed
        if (builder is EntityTypeBuilder<AuthAudit> auditBuilder)
        {
            auditBuilder.ToTable("AuthAudits");

            auditBuilder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(64);

            auditBuilder.Property(x => x.WasSuccessful)
                .IsRequired();

            auditBuilder.Property(x => x.IpAddress)
                .HasMaxLength(64);

            auditBuilder.Property(x => x.UserAgent)
                .HasMaxLength(256);

            auditBuilder.Property(x => x.FailureReason)
                .HasMaxLength(512);

            auditBuilder.HasOne(x => x.AuthUser)
                .WithMany()
                .HasForeignKey(x => x.AuthUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

public static class AuthAuditType
{
    public const string Login = "Login";
    public const string Register = "Register";
    public const string PasswordChange = "PasswordChange";
    public const string PasswordReset = "PasswordReset";
    public const string TwoFactor = "TwoFactor";
    public const string EmailConfirmation = "EmailConfirmation";
}

