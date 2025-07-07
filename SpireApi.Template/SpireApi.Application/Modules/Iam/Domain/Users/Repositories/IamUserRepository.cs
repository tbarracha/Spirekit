using SpireApi.Application.Modules.Iam.Domain.Users.Models;
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Users.Repositories;

public class IamUserRepository : BaseIamEntityRepository<IamUser>
{
    public IamUserRepository(BaseIamDbContext context) : base(context)
    {
    }

    // Add any custom queries or methods for Group here if needed
}
