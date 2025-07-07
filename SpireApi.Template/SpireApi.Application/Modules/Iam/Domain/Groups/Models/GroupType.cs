using SpireCore.API.EntityFramework.Entities.Memberships;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Models;

/// <summary>
/// Concrete implementation for a group type/category entity (e.g., team, project, organization) with Guid ID.
/// </summary>
public class GroupType : BaseGroupType<Guid>
{
}
