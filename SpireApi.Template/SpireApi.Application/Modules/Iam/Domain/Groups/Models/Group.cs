using SpireCore.API.EntityFramework.Entities.Memberships;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Models;

/// <summary>
/// Concrete implementation for a group entity (team, project, organization) with Guid IDs.
/// Supports hierarchy, type/category, and direct navigation to members.
/// </summary>
public class Group : BaseGroup<Guid, Guid, GroupMember, Guid, Guid, Guid, Guid>
{
}
