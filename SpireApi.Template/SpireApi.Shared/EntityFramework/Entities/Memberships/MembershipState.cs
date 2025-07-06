namespace SpireApi.Shared.EntityFramework.Entities.Memberships;

/// <summary>
/// The participation state of a member within a group.
/// </summary>
public enum MembershipState
{
    Pending,
    Active,
    Suspended,
    Banned
}