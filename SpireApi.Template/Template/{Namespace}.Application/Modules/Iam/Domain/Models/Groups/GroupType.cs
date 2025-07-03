// File: Application.Modules.Iam.Domain.Models.Groups.GroupType.cs
using {Namespace}.Application.Shared.Entities;

namespace {Namespace}.Application.Modules.Iam.Domain.Models.Groups;

public class GroupType : GuidEntityBy
{
    public string Name { get; set; } = default!; // e.g., "Team", "Project", "Organization"
    public string? Description { get; set; }
    public List<Group> Groups { get; set; } = new();
}

