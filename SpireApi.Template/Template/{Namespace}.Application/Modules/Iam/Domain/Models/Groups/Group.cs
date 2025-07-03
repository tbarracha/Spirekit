// File: Application.Modules.Iam.Domain.Models.Groups.Group.cs
using {Namespace}.Application.Shared.Entities;

namespace {Namespace}.Application.Modules.Iam.Domain.Models.Groups;

public class Group : GuidEntityBy
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public Guid OwnerAccountId { get; set; }

    public Guid GroupTypeId { get; set; }
    public GroupType GroupType { get; set; } = default!;

    public List<GroupAccount> Accounts { get; set; } = new();
}

