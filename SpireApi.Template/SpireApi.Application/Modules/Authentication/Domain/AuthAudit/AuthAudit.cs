using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireApi.Application.Features.Authentication.Domain.AuthUsers.Models;
using SpireCore.Abstractions.Interfaces;

namespace SpireApi.Application.Features.Authentication.Domain.AuthAudit;

public class AuthAudit : ICreatedAt
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AuthUserId { get; set; }
    public AuthUser AuthUser { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Type { get; set; } = string.Empty; // AuthAuditType.Login, etc.

    public bool WasSuccessful { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string? FailureReason { get; set; }

    public class Configuration : IEntityTypeConfiguration<AuthAudit>
    {
        public void Configure(EntityTypeBuilder<AuthAudit> builder)
        {
            builder.ToTable("AuthAudits");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(x => x.WasSuccessful)
                   .IsRequired();

            builder.Property(x => x.IpAddress)
                   .HasMaxLength(64);

            builder.Property(x => x.UserAgent)
                   .HasMaxLength(256);

            builder.Property(x => x.FailureReason)
                   .HasMaxLength(512);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasOne(x => x.AuthUser)
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
