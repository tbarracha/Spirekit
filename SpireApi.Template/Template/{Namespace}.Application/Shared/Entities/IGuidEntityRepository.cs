namespace {Namespace}.Application.Shared.Entities;

public interface IGuidEntityRepository<T> : IRepository<T, Guid> where T : GuidEntity
{

}

