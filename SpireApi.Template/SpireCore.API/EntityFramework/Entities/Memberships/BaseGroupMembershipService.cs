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
    TUserId,
    TRoleId,
    TMembershipStateId,
    TAuditId,
    TGroup,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit,
    TGroupType>
    : IGroupService<
        TGroupId,
        TGroupTypeId,
        TMemberId,
        TUserId,
        TRoleId,
        TMembershipStateId,
        TAuditId,
        TGroup,
        TGroupMember,
        TGroupMembershipState,
        TGroupMemberAudit,
        TGroupType>
    where TGroup : IGroup<TGroupId, TGroupTypeId, TGroupMember, TMemberId, TUserId, TRoleId, TMembershipStateId>
    where TGroupMember : IGroupMember<TMemberId, TGroupId, TUserId, TRoleId, TMembershipStateId>
    where TGroupMembershipState : IGroupMembershipState<TMembershipStateId, TUserId>
    where TGroupMemberAudit : IGroupMemberAudit<TAuditId, TMemberId, TGroupId, TUserId>
    where TGroupType : IGroupType<TGroupTypeId>
{


    // ----- Group management -----
    /// <inheritdoc />
    public abstract Task<TGroup> CreateGroupAsync(
        TUserId ownerUserId, string name, TGroupTypeId groupTypeId, string? description = null, TGroupId? parentGroupId = default);

    /// <inheritdoc />
    public abstract Task<TGroup?> GetGroupByIdAsync(TGroupId groupId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroup>> ListGroupsForUserAsync(TUserId userId);

    /// <inheritdoc />
    public abstract Task<TGroup?> GetOwnedGroupAsync(TUserId ownerUserId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroup>> ListGroupsByOwnerAsync(TUserId ownerUserId);

    /// <inheritdoc />
    public abstract Task<bool> UserIsGroupOwnerAsync(TGroupId groupId, TUserId userId);

    /// <inheritdoc />
    public abstract Task TransferGroupOwnershipAsync(
        TGroupId groupId,
        TUserId newOwnerUserId,
        TUserId? performedByUserId = default,
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
        TGroupId groupId, TUserId userId, TUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task RemoveMemberAsync(
        TMemberId groupMemberId, TUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task<TGroupMember?> GetGroupMemberByIdAsync(TMemberId groupMemberId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListGroupMembersAsync(TGroupId groupId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListUserGroupsAsync(TUserId userId);

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
        TMemberId groupMemberId, TRoleId roleId, TUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task RemoveRoleAsync(
        TMemberId groupMemberId, TUserId? moderatorUserId = default, string? reason = null);



    // ----- Moderation -----
    /// <inheritdoc />
    public abstract Task ModerateMemberAsync(
        TMemberId groupMemberId, MembershipState newState, TUserId? moderatorUserId = default, string? reason = null);



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
    public abstract Task<bool> UserIsGroupMemberAsync(TGroupId groupId, TUserId userId);

    /// <inheritdoc />
    public abstract Task<bool> UserHasRoleAsync(TGroupId groupId, TUserId userId, TRoleId roleId);

    /// <inheritdoc />
    public abstract Task<int> CountGroupMembersAsync(TGroupId groupId, MembershipState? participationState = null);

    /// <inheritdoc />
    public abstract Task<int> CountGroupsForUserAsync(TUserId userId);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListMembersByStateAsync(TGroupId groupId, MembershipState participationState);

    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> ListBannedMembersAsync(TGroupId groupId);



    // ----- Bulk operations -----
    /// <inheritdoc />
    public abstract Task<IReadOnlyList<TGroupMember>> AddMembersBulkAsync(
        TGroupId groupId, IEnumerable<TUserId> userIds, TUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task RemoveMembersBulkAsync(
        IEnumerable<TMemberId> groupMemberIds, TUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task AssignRoleBulkAsync(
        IEnumerable<TMemberId> groupMemberIds, TRoleId roleId, TUserId? moderatorUserId = default, string? reason = null);

    /// <inheritdoc />
    public abstract Task<DateTime?> GetMemberJoinDateAsync(TGroupId groupId, TUserId userId);

    /// <inheritdoc />
    public abstract Task<TGroupMember?> GetMemberByUserIdAsync(TGroupId groupId, TUserId userId);
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
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit,
    TGroupType>
    : BaseGroupMembershipService<
        TId, TGroupTypeId, TId, TId, TRoleId, TMembershipStateId, TId,
        TGroup, TGroupMember, TGroupMembershipState, TGroupMemberAudit, TGroupType>
    where TGroup : IGroup<TId, TGroupTypeId, TGroupMember, TId, TId, TRoleId, TMembershipStateId>
    where TGroupMember : IGroupMember<TId, TId, TId, TRoleId, TMembershipStateId>
    where TGroupMembershipState : IGroupMembershipState<TMembershipStateId, TId>
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
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit,
    TGroupType>
    : BaseGroupMembershipService<
        TId, TId, TId, TId, TRoleId, TId, TId,
        TGroup, TGroupMember, TGroupMembershipState, TGroupMemberAudit, TGroupType>
    where TGroup : IGroup<TId, TId, TGroupMember, TId, TId, TRoleId, TId>
    where TGroupMember : IGroupMember<TId, TId, TId, TRoleId, TId>
    where TGroupMembershipState : IGroupMembershipState<TId, TId>
    where TGroupMemberAudit : IGroupMemberAudit<TId, TId, TId, TId>
    where TGroupType : IGroupType<TId>
{
}
