using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpireCore.Abstractions.Interfaces;

namespace SpireCore.API.EntityFramework.Entities.Base;

public interface IEntity<TId> : IHasId<TId>, ICreatedAt, IUpdatedAt, IStateFlag
{
    void ConfigureEntity<T>(EntityTypeBuilder<T> builder) where T : class, IEntity<TId>;
}
