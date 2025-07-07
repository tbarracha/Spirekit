using SpireCore.API.EntityFramework.Entities.Memberships;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Models;

/// <summary>
/// Concrete implementation for a group member audit/moderation record with Guid IDs.
/// </summary>
public class GroupMemberAudit : BaseGroupMemberAudit<Guid, Guid, Guid, Guid>
{
}