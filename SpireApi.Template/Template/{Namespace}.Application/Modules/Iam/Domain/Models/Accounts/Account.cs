// File: Application.Modules.Iam.Domain.Models.Accounts.Account.cs
namespace {Namespace}.Application.Modules.Iam.Domain.Models.Accounts;

public class Account : GuidEntityBy
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

