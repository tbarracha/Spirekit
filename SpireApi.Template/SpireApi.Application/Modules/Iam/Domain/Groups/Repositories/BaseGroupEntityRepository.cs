using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.API.EntityFramework.Entities.Base;
using SpireCore.API.EntityFramework.Entities.Repositories;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;

public abstract class BaseGroupEntityRepository<T> : BaseAuditableEntityRepository<T, Guid, BaseIamDbContext>
    where T : class, IAuditableEntity<Guid>
{
    protected BaseGroupEntityRepository(BaseIamDbContext context) : base(context)
    {

    }
}
