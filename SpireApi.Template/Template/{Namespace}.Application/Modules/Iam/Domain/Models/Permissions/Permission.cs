// File: Application.Modules.Iam.Domain.Models.Permissions.Permission.cs
using {Namespace}.Application.Shared.Entities;

namespace {Namespace}.Application.Modules.Iam.Domain.Models.Permissions;

public class Permission : GuidEntityBy
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

