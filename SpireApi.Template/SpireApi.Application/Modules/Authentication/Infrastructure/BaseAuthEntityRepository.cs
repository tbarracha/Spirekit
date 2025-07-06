using SpireApi.Application.Modules.Authentication.Infrastructure;
using SpireApi.Shared.EntityFramework.Entities.Repositories;

namespace SpireApi.Application.Modules.Authentication.Domain.Models;

public abstract class BaseAuthEntityRepository<T> : BaseAuditableEntityRepository<T, Guid, BaseAuthDbContext> where T : BaseAuthEntity
{
    protected BaseAuthEntityRepository(BaseAuthDbContext context) : base(context)
    {

    }
}
