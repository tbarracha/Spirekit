using SpireCore.API.EntityFramework.Entities.Base;
using SpireCore.API.EntityFramework.Entities.Repositories;

namespace SpireApi.Application.Modules.Iam.Infrastructure;

public abstract class BaseIamEntityRepository<T> : BaseAuditableEntityRepository<T, Guid, BaseIamDbContext>
    where T : class, IAuditableEntity<Guid>
{
    protected BaseIamEntityRepository(BaseIamDbContext context) : base(context)
    {

    }
}
