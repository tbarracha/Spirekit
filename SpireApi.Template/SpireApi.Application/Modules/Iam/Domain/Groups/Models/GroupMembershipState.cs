using SpireCore.API.EntityFramework.Entities.Memberships;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Models;

/// <summary>
/// Concrete implementation for group membership state (suspension, ban, etc.) with Guid IDs.
/// </summary>
public class GroupMembershipState : BaseGroupMembershipState<Guid, Guid, Guid>
{
}
