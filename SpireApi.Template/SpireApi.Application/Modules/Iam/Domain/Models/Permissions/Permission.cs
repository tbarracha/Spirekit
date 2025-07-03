// File: Application.Modules.Iam.Domain.Models.Permissions.Permission.cs
using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Permissions;

public class Permission : GuidEntityBy
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
