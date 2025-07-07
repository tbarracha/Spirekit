using SpireCore.API.EntityFramework.Entities.Memberships;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Models;

/// <summary>
/// Concrete implementation for a group member entity with Guid IDs.
/// </summary>
public class GroupMember : BaseGroupMember<Guid, Guid, Guid, Guid, Guid>
{
}
