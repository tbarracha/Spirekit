
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups;

public class Group : BaseIamEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public Guid OwnerUserId { get; set; }

    public Guid GroupTypeId { get; set; }
    public GroupType GroupType { get; set; } = default!;

    public List<GroupMember> Members { get; set; } = new();
}
