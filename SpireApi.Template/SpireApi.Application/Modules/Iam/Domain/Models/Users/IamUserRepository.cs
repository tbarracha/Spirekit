using SpireApi.Application.Modules.Iam.Domain.Models.Users;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Users.Repositories;

public class IamUserRepository : BaseIamEntityRepository<IamUser>
{
    public IamUserRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}
