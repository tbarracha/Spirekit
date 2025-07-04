using SpireApi.Shared.EntityFramework.Repositories;

namespace SpireApi.Application.Modules.Iam.Infrastructure;

public abstract class BaseIamEntityRepository<T> : BaseAuditableEntityRepository<T, Guid, BaseIamDbContext> where T : BaseIamEntity
{
    protected BaseIamEntityRepository(BaseIamDbContext context) : base(context)
    {

    }
}
