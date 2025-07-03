using SpireCore.API.EntityFramework.Repositories;

namespace SpireApi.Application.Shared.Entities;

public class GuidEntityRepository<T> : BaseRepository<T, Guid, GuidEntityDbContext>, IGuidEntityRepository<T>
    where T : GuidEntity
{
    public GuidEntityRepository(GuidEntityDbContext context) : base(context) { }
}
