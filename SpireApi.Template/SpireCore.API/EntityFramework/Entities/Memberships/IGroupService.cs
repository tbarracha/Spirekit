namespace SpireCore.API.EntityFramework.Entities.Memberships;

/// <summary>
/// Generic service interface for managing groups, group members, membership states, and audit records,
/// supporting distinct ID types for each entity and generic RoleId.
/// Includes support for group ownership management.
/// </summary>
/// <typeparam name="TGroupId">ID type for Group entities.</typeparam>
/// <typeparam name="TGroupTypeId">ID type for GroupType entities.</typeparam>
/// <typeparam name="TMemberId">ID type for GroupMember entities.</typeparam>
/// <typeparam name="TUserId">ID type for User entities.</typeparam>
/// <typeparam name="TRoleId">ID type for Role entities.</typeparam>
/// <typeparam name="TMembershipStateId">ID type for GroupMembershipState entities.</typeparam>
/// <typeparam name="TAuditId">ID type for GroupMemberAudit entities.</typeparam>
/// <typeparam name="TGroup">Group entity type.</typeparam>
/// <typeparam name="TGroupMember">GroupMember entity type.</typeparam>
/// <typeparam name="TGroupMembershipState">GroupMembershipState entity type.</typeparam>
/// <typeparam name="TGroupMemberAudit">GroupMemberAudit entity type.</typeparam>
/// <typeparam name="TGroupType">GroupType entity type.</typeparam>
public interface IGroupService<
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

    /// <summary>
    /// Creates a new group with the specified owner, name, type, optional description, and optional parent group.
    /// </summary>
    Task<TGroup> CreateGroupAsync(
        TUserId ownerUserId,
        string name,
        TGroupTypeId groupTypeId,
        string? description = null,
        TGroupId? parentGroupId = default);

    /// <summary>
    /// Retrieves a group by its unique identifier.
    /// </summary>
    Task<TGroup?> GetGroupByIdAsync(TGroupId groupId);

    /// <summary>
    /// Lists all groups that the given user is a member of.
    /// </summary>
    Task<IReadOnlyList<TGroup>> ListGroupsForUserAsync(TUserId userId);

    /// <summary>
    /// Gets the group owned by the specified user, if any.
    /// </summary>
    Task<TGroup?> GetOwnedGroupAsync(TUserId ownerUserId);

    /// <summary>
    /// Lists all groups owned by the specified user.
    /// </summary>
    Task<IReadOnlyList<TGroup>> ListGroupsByOwnerAsync(TUserId ownerUserId);

    /// <summary>
    /// Checks if the specified user is the owner of the given group.
    /// </summary>
    Task<bool> UserIsGroupOwnerAsync(TGroupId groupId, TUserId userId);

    /// <summary>
    /// Transfers group ownership to another user.
    /// Optionally, records the user who performed the transfer and a reason.
    /// </summary>
    Task TransferGroupOwnershipAsync(TGroupId groupId, TUserId newOwnerUserId, TUserId? performedByUserId = default, string? reason = null);


    // ----- Group type queries -----

    /// <summary>
    /// Retrieves a group type by its unique identifier.
    /// </summary>
    Task<TGroupType?> GetGroupTypeByIdAsync(TGroupTypeId typeId);

    /// <summary>
    /// Lists all available group types.
    /// </summary>
    Task<IReadOnlyList<TGroupType>> ListGroupTypesAsync();


    // ----- Hierarchy queries -----

    /// <summary>
    /// Gets the child groups for the specified parent group.
    /// </summary>
    Task<IReadOnlyList<TGroup>> GetChildGroupsAsync(TGroupId parentGroupId);

    /// <summary>
    /// Gets the parent group of the specified group, if any.
    /// </summary>
    Task<TGroup?> GetParentGroupAsync(TGroupId groupId);


    // ----- Member management -----

    /// <summary>
    /// Adds a user as a member to the specified group.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task<TGroupMember> AddMemberAsync(
        TGroupId groupId,
        TUserId userId,
        TUserId? moderatorUserId = default,
        string? reason = null);

    /// <summary>
    /// Removes a group member by their member ID.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task RemoveMemberAsync(
        TMemberId groupMemberId,
        TUserId? moderatorUserId = default,
        string? reason = null);

    /// <summary>
    /// Retrieves a group member by their unique member ID.
    /// </summary>
    Task<TGroupMember?> GetGroupMemberByIdAsync(TMemberId groupMemberId);

    /// <summary>
    /// Lists all members in the specified group.
    /// </summary>
    Task<IReadOnlyList<TGroupMember>> ListGroupMembersAsync(TGroupId groupId);

    /// <summary>
    /// Lists all group memberships for the specified user.
    /// </summary>
    Task<IReadOnlyList<TGroupMember>> ListUserGroupsAsync(TUserId userId);

    /// <summary>
    /// Lists all members of the group who are moderators.
    /// </summary>
    Task<IReadOnlyList<TGroupMember>> ListGroupModeratorsAsync(TGroupId groupId);


    // ----- Membership state management -----

    /// <summary>
    /// Gets a group membership state by its unique state ID.
    /// </summary>
    Task<TGroupMembershipState?> GetGroupMembershipStateAsync(TMembershipStateId stateId);

    /// <summary>
    /// Lists all membership state records for the specified group member.
    /// </summary>
    Task<IReadOnlyList<TGroupMembershipState>> ListMembershipStatesForMemberAsync(TMemberId groupMemberId);


    // ----- Role management -----

    /// <summary>
    /// Assigns a role to the specified group member.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task AssignRoleAsync(
        TMemberId groupMemberId,
        TRoleId roleId,
        TUserId? moderatorUserId = default,
        string? reason = null);

    /// <summary>
    /// Removes the role from the specified group member.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task RemoveRoleAsync(
        TMemberId groupMemberId,
        TUserId? moderatorUserId = default,
        string? reason = null);


    // ----- Moderation -----

    /// <summary>
    /// Changes the moderation state of a group member (e.g., suspend, ban, etc).
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task ModerateMemberAsync(
        TMemberId groupMemberId,
        MembershipState newState,
        TUserId? moderatorUserId = default,
        string? reason = null);


    // ----- Audits -----

    /// <summary>
    /// Lists all audit records for the specified group member.
    /// </summary>
    Task<IReadOnlyList<TGroupMemberAudit>> ListMemberAuditsAsync(TMemberId groupMemberId);

    /// <summary>
    /// Lists all audit records for all members of the specified group.
    /// </summary>
    Task<IReadOnlyList<TGroupMemberAudit>> ListGroupAuditsAsync(TGroupId groupId);

    /// <summary>
    /// Gets the latest audit record for the specified group member.
    /// </summary>
    Task<TGroupMemberAudit?> GetLatestMemberAuditAsync(TMemberId groupMemberId);


    // ----- Utility -----

    /// <summary>
    /// Checks if a group exists by its unique group ID.
    /// </summary>
    Task<bool> GroupExistsAsync(TGroupId groupId);

    /// <summary>
    /// Checks if the specified user is a member of the given group.
    /// </summary>
    Task<bool> UserIsGroupMemberAsync(TGroupId groupId, TUserId userId);

    /// <summary>
    /// Checks if the specified user has the given role in the group.
    /// </summary>
    Task<bool> UserHasRoleAsync(TGroupId groupId, TUserId userId, TRoleId roleId);

    /// <summary>
    /// Returns the number of members in the group, optionally filtered by participation state.
    /// </summary>
    Task<int> CountGroupMembersAsync(TGroupId groupId, MembershipState? participationState = null);

    /// <summary>
    /// Returns the number of groups the specified user is a member of.
    /// </summary>
    Task<int> CountGroupsForUserAsync(TUserId userId);

    /// <summary>
    /// Lists all members of the group with the specified participation state (e.g., active, banned).
    /// </summary>
    Task<IReadOnlyList<TGroupMember>> ListMembersByStateAsync(TGroupId groupId, MembershipState participationState);

    /// <summary>
    /// Lists all banned members in the specified group.
    /// </summary>
    Task<IReadOnlyList<TGroupMember>> ListBannedMembersAsync(TGroupId groupId);


    // ----- Bulk operations -----

    /// <summary>
    /// Adds multiple users as members to the specified group in bulk.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task<IReadOnlyList<TGroupMember>> AddMembersBulkAsync(
        TGroupId groupId,
        IEnumerable<TUserId> userIds,
        TUserId? moderatorUserId = default,
        string? reason = null);

    /// <summary>
    /// Removes multiple group members by their member IDs in bulk.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task RemoveMembersBulkAsync(
        IEnumerable<TMemberId> groupMemberIds,
        TUserId? moderatorUserId = default,
        string? reason = null);

    /// <summary>
    /// Assigns a role to multiple group members in bulk.
    /// Optionally records the moderator and a reason.
    /// </summary>
    Task AssignRoleBulkAsync(
        IEnumerable<TMemberId> groupMemberIds,
        TRoleId roleId,
        TUserId? moderatorUserId = default,
        string? reason = null);

    /// <summary>
    /// Gets the join date for a user in the specified group, if the user is a member.
    /// </summary>
    Task<DateTime?> GetMemberJoinDateAsync(TGroupId groupId, TUserId userId);

    /// <summary>
    /// Retrieves the group member entity for the specified user and group, if present.
    /// </summary>
    Task<TGroupMember?> GetMemberByUserIdAsync(TGroupId groupId, TUserId userId);
}


/// <summary>
/// Convenience interface for when all ID types are the same (e.g., Guid), except RoleId, GroupTypeId, and MembershipStateId.
/// </summary>
public interface IGroupService<
    TId,
    TRoleId,
    TGroupTypeId,
    TMembershipStateId,
    TGroup,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit,
    TGroupType>
    : IGroupService<
        TId, TGroupTypeId, TId, TId, TRoleId, TMembershipStateId, TId,
        TGroup, TGroupMember, TGroupMembershipState, TGroupMemberAudit, TGroupType>
    where TGroup : IGroup<TId, TGroupTypeId, TGroupMember, TId, TId, TRoleId, TMembershipStateId>
    where TGroupMember : IGroupMember<TId, TId, TId, TRoleId, TMembershipStateId>
    where TGroupMembershipState : IGroupMembershipState<TMembershipStateId, TId>
    where TGroupMemberAudit : IGroupMemberAudit<TId, TId, TId, TId>
    where TGroupType : IGroupType<TGroupTypeId>
{ }

/// <summary>
/// Convenience interface for when all ID types are the same (e.g., Guid).
/// </summary>
public interface IGroupService<
    TId,
    TRoleId,
    TGroup,
    TGroupMember,
    TGroupMembershipState,
    TGroupMemberAudit,
    TGroupType>
    : IGroupService<
        TId, TId, TId, TId, TRoleId, TId, TId,
        TGroup, TGroupMember, TGroupMembershipState, TGroupMemberAudit, TGroupType>
    where TGroup : IGroup<TId, TId, TGroupMember, TId, TId, TRoleId, TId>
    where TGroupMember : IGroupMember<TId, TId, TId, TRoleId, TId>
    where TGroupMembershipState : IGroupMembershipState<TId, TId>
    where TGroupMemberAudit : IGroupMemberAudit<TId, TId, TId, TId>
    where TGroupType : IGroupType<TId>
{ }
