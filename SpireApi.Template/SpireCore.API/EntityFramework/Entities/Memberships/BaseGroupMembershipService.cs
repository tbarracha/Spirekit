namespace SpireCore.API.EntityFramework.Entities.Memberships;

/// <summary>
/// Abstract base class for a group membership service, providing all required operations
/// as abstract methods to be implemented in concrete subclasses.
/// Now includes group ownership management methods.
/// </summary>
public abstract class BaseGroupMembershipService<
    TGroupId,
    TGroupTypeId,
    TMemberId,
    TModeratorUserId,
    TRoleId,
    TMembershipStateId,
    TAuditId,
    TGroup,
    TGroupType,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit
    >
    : IGroupService<
    TGroupId,
    TGroupTypeId,
    TMemberId,
    TModeratorUserId,
    TRoleId,
    TMembershipStateId,
    TAuditId,
    TGroup,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit,
    TGroupType>
    where TGroup : IGroup<TGroupId, TGroupTypeId, TGroupMember, TMemberId, TModeratorUserId, TRoleId, TMembershipStateId>
    where TGroupMember : IGroupMember<TMemberId, TGroupId, TModeratorUserId, TRoleId, TMembershipStateId>
    where TGroupMembershipState : IGroupMembershipState<TMembershipStateId, TMemberId, TModeratorUserId>
    where TGroupMemberAudit : IGroupMemberAudit<TAuditId, TMemberId, TGroupId, TModeratorUserId>
    where TGroupType : IGroupType<TGroupTypeId>
{


    // ----- Group management -----
    /// <inheritdoc />
    public abstract Task<TGroup> CreateGroupAsync(
        TModeratorUserId ownerUserId, string name, TGroupTypeId groupTypeId, string? description = null, TGroupId? parentGroupId = default);

    /// <inheritdoc />
    public abstract Task<TGroup?> GetGroupByIdAsync(TGroupId groupId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroup>> ListGroupsForUserAsync(TModeratorUserId userId);

    /// <inheritdoc />
    public abstract Task<TGroup?> GetOwnedGroupAsync(TModeratorUserId ownerUserId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroup>> ListGroupsByOwnerAsync(TModeratorUserId ownerUserId);

    /// <inheritdoc />
    public abstract Task<bool> UserIsGroupOwnerAsync(TGroupId groupId, TModeratorUserId userId);

    /// <inheritdoc />
    public abstract Task TransferGroupOwnershipAsync(
        TGroupId groupId,
        TModeratorUserId newOwnerUserId,
        TModeratorUserId? performedByUserId = default,
        string? reason = null);



    // ----- Group type queries -----
    /// <inheritdoc />
    public abstract Task<TGroupType?> GetGroupTypeByIdAsync(TGroupTypeId typeId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupType>> ListGroupTypesAsync();



    // ----- Hierarchy queries -----
    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroup>> GetChildGroupsAsync(TGroupId parentGroupId);

    /// <inheritdoc />
    public abstract Task<TGroup?> GetParentGroupAsync(TGroupId groupId);



    // ----- Member management -----
    /// <inheritdoc />
    public abstract Task<TGroupMember> AddMemberAsync(
        TGroupId groupId, TModeratorUserId userId, TModeratorUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task RemoveMemberAsync(
        TMemberId groupMemberId, TModeratorUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task<TGroupMember?> GetGroupMemberByIdAsync(TMemberId groupMemberId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListGroupMembersAsync(TGroupId groupId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListUserGroupsAsync(TModeratorUserId userId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListGroupModeratorsAsync(TGroupId groupId);



    // ----- Membership state management -----
    /// <inheritdoc />
    public abstract Task<TGroupMembershipState?> GetGroupMembershipStateAsync(TMembershipStateId stateId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMembershipState>> ListMembershipStatesForMemberAsync(TMemberId groupMemberId);



    // ----- Role management -----
    /// <inheritdoc />
    public abstract Task AssignRoleAsync(
        TMemberId groupMemberId, TRoleId roleId, TModeratorUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task RemoveRoleAsync(
        TMemberId groupMemberId, TModeratorUserId? moderatorUserId = default, string? reason = null);



    // ----- Moderation -----
    /// <inheritdoc />
    public abstract Task ModerateMemberAsync(
        TMemberId groupMemberId, MembershipState newState, TModeratorUserId? moderatorUserId = default, string? reason = null);



    // ----- Audits -----
    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMemberAudit>> ListMemberAuditsAsync(TMemberId groupMemberId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMemberAudit>> ListGroupAuditsAsync(TGroupId groupId);

    /// <inheritdoc />
    public abstract Task<TGroupMemberAudit?> GetLatestMemberAuditAsync(TMemberId groupMemberId);



    // ----- Utility -----
    /// <inheritdoc />
    public abstract Task<bool> GroupExistsAsync(TGroupId groupId);

    /// <inheritdoc />
    public abstract Task<bool> UserIsGroupMemberAsync(TGroupId groupId, TModeratorUserId userId);

    /// <inheritdoc />
    public abstract Task<bool> UserHasRoleAsync(TGroupId groupId, TModeratorUserId userId, TRoleId roleId);

    /// <inheritdoc />
    public abstract Task<int> CountGroupMembersAsync(TGroupId groupId, MembershipState? participationState = null);

    /// <inheritdoc />
    public abstract Task<int> CountGroupsForUserAsync(TModeratorUserId userId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListMembersByStateAsync(TGroupId groupId, MembershipState participationState);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListBannedMembersAsync(TGroupId groupId);



    // ----- Bulk operations -----
    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> AddMembersBulkAsync(
        TGroupId groupId, IEnumerable<TModeratorUserId> userIds, TModeratorUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task RemoveMembersBulkAsync(
        IEnumerable<TMemberId> groupMemberIds, TModeratorUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task AssignRoleBulkAsync(
        IEnumerable<TMemberId> groupMemberIds, TRoleId roleId, TModeratorUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task<DateTime?> GetMemberJoinDateAsync(TGroupId groupId, TModeratorUserId userId);

    /// <inheritdoc />
    public abstract Task<TGroupMember?> GetMemberByUserIdAsync(TGroupId groupId, TModeratorUserId userId);
}


/// <summary>
/// Convenience base class for when all ID types are the same (e.g., Guid), except RoleId, GroupTypeId, and MembershipStateId.
/// </summary>
public abstract class BaseGroupMembershipService<
    TId,
    TRoleId,
    TGroupTypeId,
    TMembershipStateId,
    TGroup,
    TGroupType,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit
    >
    : BaseGroupMembershipService<
        TId, TGroupTypeId, TId, TId, TRoleId, TMembershipStateId, TId,
        TGroup, TGroupType, TGroupMember, TGroupMembershipState, TGroupMemberAudit>
    where TGroup : IGroup<TId, TGroupTypeId, TGroupMember, TId, TId, TRoleId, TMembershipStateId>
    where TGroupMember : IGroupMember<TId, TId, TId, TRoleId, TMembershipStateId>
    where TGroupMembershipState : IGroupMembershipState<TMembershipStateId, TId, TId>
    where TGroupMemberAudit : IGroupMemberAudit<TId, TId, TId, TId>
    where TGroupType : IGroupType<TGroupTypeId>
{
}

/// <summary>
/// Convenience base class for when all ID types are the same (e.g., Guid).
/// </summary>
public abstract class BaseGroupMembershipService<
    TId,
    TRoleId,
    TGroup,
    TGroupType,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit
    >
    : BaseGroupMembershipService<
        TId, TId, TId, TId, TRoleId, TId, TId,
        TGroup, TGroupType, TGroupMember, TGroupMembershipState, TGroupMemberAudit>
    where TGroup : IGroup<TId, TId, TGroupMember, TId, TId, TRoleId, TId>
    where TGroupMember : IGroupMember<TId, TId, TId, TRoleId, TId>
    where TGroupMembershipState : IGroupMembershipState<TId, TId, TId>
    where TGroupMemberAudit : IGroupMemberAudit<TId, TId, TId, TId>
    where TGroupType : IGroupType<TId>
{
}
