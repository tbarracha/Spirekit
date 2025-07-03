// File: Application.Modules.Iam.Domain.Models.Roles.Role.cs
using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Roles;

public class Role : GuidEntityBy
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid AccountId { get; set; }
}
