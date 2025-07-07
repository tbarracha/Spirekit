using SpireCore.Abstractions.Interfaces;
using SpireCore.API.EntityFramework.Entities.Base;

namespace SpireCore.API.EntityFramework.Entities.Memberships;

/// <summary>
/// Abstraction for a group entity (team, project, organization, etc.).
/// Supports hierarchical relationships via parent/child groups and always exposes members.
/// </summary>
/// <typeparam name="TGroupId">The unique identifier type for the group.</typeparam>
/// <typeparam name="TGroupTypeId">The unique identifier type for the group type.</typeparam>
/// <typeparam name="TMember">The group member interface type.</typeparam>
/// <typeparam name="TMemberId">The unique identifier type for a group member.</typeparam>
/// <typeparam name="TUserId">The unique identifier type for a user.</typeparam>
/// <typeparam name="TRoleId">The unique identifier type for a role.</typeparam>
/// <typeparam name="TMembershipStateId">The unique identifier type for a membership state.</typeparam>
public interface IGroup<
    TGroupId,
    TGroupTypeId,
    TMember,
    TMemberId,
    TUserId,
    TRoleId,
    TMembershipStateId>
    : IAuditableEntity<TGroupId>
    where TMember : IGroupMember<TMemberId, TGroupId, TUserId, TRoleId, TMembershipStateId>
{
    /// <summary>
    /// The display name of the group.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// An optional description for the group.
    /// </summary>
    string? Description { get; set; }

    /// <summary>
    /// The identifier of the group's type or category.
    /// </summary>
    TGroupTypeId GroupTypeId { get; set; }

    /// <summary>
    /// Navigation property for the group's type/category.
    /// </summary>
    IGroupType<TGroupTypeId> GroupType { get; set; }

    /// <summary>
    /// Collection of members belonging to the group.
    /// </summary>
    ICollection<TMember> Members { get; set; }

    /// <summary>
    /// The unique identifier of the user who owns this group.
    /// </summary>
    TUserId OwnerUserId { get; set; }

    /// <summary>
    /// The optional identifier of the group's parent group, if this group is nested.
    /// </summary>
    TGroupId? ParentGroupId { get; set; }

    /// <summary>
    /// Navigation property to the parent group, if any.
    /// </summary>
    IGroup<TGroupId, TGroupTypeId, TMember, TMemberId, TUserId, TRoleId, TMembershipStateId>? ParentGroup { get; set; }

    /// <summary>
    /// Collection navigation for the group's child groups, supporting hierarchical group structures.
    /// </summary>
    ICollection<IGroup<TGroupId, TGroupTypeId, TMember, TMemberId, TUserId, TRoleId, TMembershipStateId>> ChildGroups { get; set; }
}

/// <summary>
/// Represents the type or category of a group, such as Team, Project, or Organization.
/// </summary>
/// <typeparam name="TGroupTypeId">The unique identifier type for the group type.</typeparam>
public interface IGroupType<TGroupTypeId> : IAuditableEntity<TGroupTypeId>, IIsDefault
{
    /// <summary>
    /// The display name of the group type.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// An optional description of the group type.
    /// </summary>
    string? Description { get; set; }
}

/// <summary>
/// Abstraction for a member entity within a group.
/// </summary>
/// <typeparam name="TMemberId">The unique identifier type for the member.</typeparam>
/// <typeparam name="TGroupId">The unique identifier type for the group.</typeparam>
/// <typeparam name="TModeratorUserId">The unique identifier type for the user.</typeparam>
/// <typeparam name="TRoleId">The unique identifier type for the role.</typeparam>
/// <typeparam name="TMembershipStateId">The unique identifier type for the membership state.</typeparam>
public interface IGroupMember<TMemberId, TGroupId, TModeratorUserId, TRoleId, TMembershipStateId> : IAuditableEntity<TMemberId>
{
    /// <summary>
    /// The unique identifier of the group this member belongs to.
    /// </summary>
    TGroupId GroupId { get; set; }

    /// <summary>
    /// The unique identifier of the user who is the member.
    /// </summary>
    TModeratorUserId UserId { get; set; }

    /// <summary>
    /// The unique identifier of the role assigned to the member; can be null if role-less memberships are allowed.
    /// </summary>
    TRoleId? RoleId { get; set; }

    /// <summary>
    /// The date and time the member joined the group.
    /// </summary>
    DateTime JoinedAt { get; set; }

    /// <summary>
    /// FK to the current membership-state record.
    /// </summary>
    TMembershipStateId StateId { get; set; }

    /// <summary>
    /// Navigation property to the member's current membership state.
    /// </summary>
    IGroupMembershipState<TMembershipStateId, TMemberId, TModeratorUserId> MembershipState { get; set; }
}

/// <summary>
/// Encapsulates the moderation and participation status of a group member, such as active, suspended, or banned.
/// </summary>
/// <typeparam name="TStateId">The unique identifier type for the membership state.</typeparam>
/// <typeparam name="TModeratorUserId">The unique identifier type for the user (moderator).</typeparam>
public interface IGroupMembershipState<TStateId, TMemberId, TModeratorUserId> : IAuditableEntity<TStateId>, IIsDefault
{
    TMemberId GroupMemberId { get; set; }

    /// <summary>
    /// The current membership state (e.g., Active, Suspended, Banned).
    /// </summary>
    MembershipState State { get; set; }

    /// <summary>
    /// The date and time the member was suspended, if applicable.
    /// </summary>
    DateTime? SuspendedAt { get; set; }

    /// <summary>
    /// The date and time when a suspension ends, if set.
    /// </summary>
    DateTime? SuspensionEndsAt { get; set; }

    /// <summary>
    /// The reason for suspension, if suspended.
    /// </summary>
    string? SuspensionReason { get; set; }

    /// <summary>
    /// The date and time the member was banned, if applicable.
    /// </summary>
    DateTime? BannedAt { get; set; }

    /// <summary>
    /// The reason for a ban, if banned.
    /// </summary>
    string? BanReason { get; set; }

    /// <summary>
    /// The identifier of the moderator who applied the membership change.
    /// </summary>
    TModeratorUserId? ModeratedByUserId { get; set; }
}

/// <summary>
/// Abstraction for a moderation or audit record for a group member, recording state changes, bans, suspensions, and other moderation events.
/// </summary>
/// <typeparam name="TAuditId">The unique identifier type for the audit record.</typeparam>
/// <typeparam name="TMemberId">The unique identifier type for the member.</typeparam>
/// <typeparam name="TGroupId">The unique identifier type for the group.</typeparam>
/// <typeparam name="TModeratorUserId">The unique identifier type for the moderator user.</typeparam>
public interface IGroupMemberAudit<TAuditId, TMemberId, TGroupId, TModeratorUserId> : IAuditableEntity<TAuditId>
{
    /// <summary>
    /// The unique identifier of the affected group member.
    /// </summary>
    TMemberId GroupMemberUserId { get; set; }

    /// <summary>
    /// The unique identifier of the group where the action occurred.
    /// </summary>
    TGroupId GroupId { get; set; }

    /// <summary>
    /// Type of audit action that applied to the group member.
    /// </summary>
    GroupMemberAuditType Action { get; set; }

    /// <summary>
    /// The new state applied to the group member as a result of moderation.
    /// </summary>
    MembershipState NewState { get; set; }

    /// <summary>
    /// The date and time when the moderation or audit action was applied.
    /// </summary>
    DateTime ChangedAt { get; set; }

    /// <summary>
    /// The reason for the moderation or audit action, if provided.
    /// </summary>
    string? Reason { get; set; }

    /// <summary>
    /// The unique identifier of the moderator who performed the action, if applicable.
    /// </summary>
    TModeratorUserId? PerformedByUserId { get; set; }
}
