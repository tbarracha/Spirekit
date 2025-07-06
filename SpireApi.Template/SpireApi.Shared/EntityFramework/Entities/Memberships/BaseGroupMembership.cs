
namespace SpireApi.Shared.EntityFramework.Entities.Memberships;

/// <summary>
/// Abstract base class for a group type/category entity (e.g., team, project, organization).
/// </summary>
public abstract class BaseGroupType<TGroupTypeId> : BaseAuditableEntity<TGroupTypeId>, IGroupType<TGroupTypeId>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

/// <summary>
/// Abstract base class for a group entity (team, project, organization),
/// supporting hierarchy, type/category, ownership, and direct navigation to members.
/// </summary>
public abstract class BaseGroup<
    TGroupId,
    TGroupTypeId,
    TGroupMember,
    TMemberId,
    TUserId,
    TRoleId,
    TMembershipStateId>
    : BaseAuditableEntity<TGroupId>,
      IGroup<TGroupId, TGroupTypeId, TGroupMember, TMemberId, TUserId, TRoleId, TMembershipStateId>
    where TGroupMember : IGroupMember<TMemberId, TGroupId, TUserId, TRoleId, TMembershipStateId>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public required TGroupTypeId GroupTypeId { get; set; }
    public IGroupType<TGroupTypeId> GroupType { get; set; } = default!;

    public ICollection<TGroupMember> Members { get; set; } = new List<TGroupMember>();

    /// <summary>
    /// The unique identifier of the user who owns this group.
    /// </summary>
    public required TUserId OwnerId { get; set; }

    // Hierarchical support
    public TGroupId? ParentGroupId { get; set; }
    public IGroup<TGroupId, TGroupTypeId, TGroupMember, TMemberId, TUserId, TRoleId, TMembershipStateId>? ParentGroup { get; set; }
    public ICollection<IGroup<TGroupId, TGroupTypeId, TGroupMember, TMemberId, TUserId, TRoleId, TMembershipStateId>> ChildGroups { get; set; }
        = new List<IGroup<TGroupId, TGroupTypeId, TGroupMember, TMemberId, TUserId, TRoleId, TMembershipStateId>>();
}

/// <summary>
/// Abstract base class for a group membership state (suspension, ban, etc.).
/// </summary>
public abstract class BaseGroupMembershipState<TMembershipStateId, TUserId>
    : BaseAuditableEntity<TMembershipStateId>,
      IGroupMembershipState<TMembershipStateId, TUserId>
{
    public MembershipState State { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public DateTime? SuspensionEndsAt { get; set; }
    public string? SuspensionReason { get; set; }
    public DateTime? BannedAt { get; set; }
    public string? BanReason { get; set; }
    public TUserId? ModeratedByUserId { get; set; }
}

/// <summary>
/// Abstract base class for a group member entity.
/// </summary>
public abstract class BaseGroupMember<TMemberId, TGroupId, TUserId, TRoleId, TMembershipStateId>
    : BaseAuditableEntity<TMemberId>,
      IGroupMember<TMemberId, TGroupId, TUserId, TRoleId, TMembershipStateId>
{
    public required TGroupId GroupId { get; set; }
    public required TUserId UserId { get; set; }
    public TRoleId? RoleId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public MembershipState? State { get; set; }
    public IGroupMembershipState<TMembershipStateId, TUserId> MembershipState { get; set; } = default!;
}

/// <summary>
/// Abstract base class for a moderation or audit record for a group member.
/// </summary>
public abstract class BaseGroupMemberAudit<TAuditId, TMemberId, TGroupId, TUserId>
    : BaseAuditableEntity<TAuditId>,
      IGroupMemberAudit<TAuditId, TMemberId, TGroupId, TUserId>
{
    public required TMemberId MemberId { get; set; }
    public required TGroupId GroupId { get; set; }
    public MembershipState NewState { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Reason { get; set; }
    public TUserId? ModeratorUserId { get; set; }
}
