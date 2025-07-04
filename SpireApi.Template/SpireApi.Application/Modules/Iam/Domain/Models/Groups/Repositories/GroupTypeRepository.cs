using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

public class GroupTypeRepository : BaseIamEntityRepository<GroupType>
{
    public GroupTypeRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}
