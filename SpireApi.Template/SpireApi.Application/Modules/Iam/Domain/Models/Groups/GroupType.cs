// File: Application.Modules.Iam.Domain.Models.Groups.GroupType.cs
using SpireApi.Application.Shared.Entities;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups;

public class GroupType : GuidEntityBy
{
    public string Name { get; set; } = default!; // e.g., "Team", "Project", "Organization"
    public string? Description { get; set; }
    public List<Group> Groups { get; set; } = new();
}
