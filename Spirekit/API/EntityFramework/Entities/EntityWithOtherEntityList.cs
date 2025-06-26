
namespace Spirekit.API.EntityFramework.Entities;

public class EntityWithOtherEntityList<TEntity, TWithEntity>
{
    public TEntity entity { get; set; } = default!;
    public List<TEntity> entities { get; set; } = default!;
}
