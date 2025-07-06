using SpireApi.Shared.EntityFramework.Entities.Memberships;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups;

/// <summary>
/// Concrete implementation for a group type/category entity (e.g., team, project, organization) with Guid ID.
/// </summary>
public class GroupType : BaseGroupType<Guid>
{
}

/// <summary>
/// Concrete implementation for group membership state (suspension, ban, etc.) with Guid IDs.
/// </summary>
public class GroupMembershipState : BaseGroupMembershipState<Guid, Guid>
{
}

/// <summary>
/// Concrete implementation for a group member entity with Guid IDs.
/// </summary>
public class GroupMember : BaseGroupMember<Guid, Guid, Guid, Guid, Guid>
{
}

/// <summary>
/// Concrete implementation for a group member audit/moderation record with Guid IDs.
/// </summary>
public class GroupMemberAudit : BaseGroupMemberAudit<Guid, Guid, Guid, Guid>
{
}

/// <summary>
/// Concrete implementation for a group entity (team, project, organization) with Guid IDs.
/// Supports hierarchy, type/category, and direct navigation to members.
/// </summary>
public class Group : BaseGroup<Guid, Guid, GroupMember, Guid, Guid, Guid, Guid>
{
}