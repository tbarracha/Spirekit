using {Namespace}.Shared.EntityFramework.Repositories;

namespace {Namespace}.Application.Shared.Entities;

public class GuidEntityRepository<T> : BaseRepository<T, Guid, GuidEntityDbContext>, IGuidEntityRepository<T>
    where T : GuidEntity
{
    public GuidEntityRepository(GuidEntityDbContext context) : base(context) { }
}

