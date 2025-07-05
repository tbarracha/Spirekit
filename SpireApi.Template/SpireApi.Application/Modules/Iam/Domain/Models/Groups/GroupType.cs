using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.Attributes.NormalizeFrom;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups;

public class GroupType : BaseIamEntity
{
    public string Name { get; set; } = default!; // e.g., "Team", "Project", "Organization"

    [NormalizedFrom(nameof(Name))]
    public string NormalizedName { get; set; } = default!;
    public string? Description { get; set; }

    public List<Group> Groups { get; set; } = new();
}
