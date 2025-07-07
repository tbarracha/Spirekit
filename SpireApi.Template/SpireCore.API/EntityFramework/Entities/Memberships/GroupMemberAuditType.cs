
namespace SpireCore.API.EntityFramework.Entities.Memberships;

/// <summary>
/// Enumerates the kinds of audit actions that can be applied to a group member.
/// Extend or override these values in your own domain model if additional
/// scenarios are required.
/// </summary>
public enum GroupMemberAuditType : byte
{
    /// <summary>Member joined the group.</summary>
    Join = 0,

    /// <summary>Member left the group voluntarily or was removed.</summary>
    Leave = 1,

    /// <summary>Member’s role was changed (assigned, updated, or removed).</summary>
    RoleChanged = 2,

    /// <summary>Member was placed in a suspended state.</summary>
    Suspended = 3,

    /// <summary>Suspension on the member was lifted.</summary>
    Unsuspended = 4,

    /// <summary>Member was permanently banned.</summary>
    Banned = 5,

    /// <summary>Ban on the member was lifted.</summary>
    Unbanned = 6,

    OwnershipTransferred = 7,

    StateChanged = 8
}
