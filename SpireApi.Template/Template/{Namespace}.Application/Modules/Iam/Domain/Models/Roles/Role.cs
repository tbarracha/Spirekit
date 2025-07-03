// File: Application.Modules.Iam.Domain.Models.Roles.Role.cs
using {Namespace}.Application.Shared.Entities;

namespace {Namespace}.Application.Modules.Iam.Domain.Models.Roles;

public class Role : GuidEntityBy
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid AccountId { get; set; }
}

