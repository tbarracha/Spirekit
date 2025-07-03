

using SpireCore.Constants;

namespace SpireCore.API.EntityFramework.Repositories;

public class RepoEntity<TId> : IRepoEntity<TId>
{
    public virtual TId Id { get; set; } = default!;
    public DateTime CreatedAt  { get; set; } = DateTime.Now;
    public DateTime UpdatedAt  { get; set; }
    public string StateFlag { get; set; } = StateFlags.ACTIVE;
}
