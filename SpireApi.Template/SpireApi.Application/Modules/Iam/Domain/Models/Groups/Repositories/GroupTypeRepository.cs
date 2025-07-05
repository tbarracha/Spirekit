using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireCore.Attributes.NormalizeFrom;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

public class GroupTypeRepository : BaseIamEntityRepository<GroupType>
{
    public GroupTypeRepository(BaseIamDbContext context) : base(context)
    {
    }

    public override Task<GroupType> AddAsync(GroupType entity)
    {
        NormalizationHelper.ApplyNormalization(entity);
        return base.AddAsync(entity);
    }

    public override Task<IReadOnlyList<GroupType>> AddRangeAsync(IEnumerable<GroupType> entities)
    {
        var list = entities.ToList();
        foreach (var entity in list)
            NormalizationHelper.ApplyNormalization(entity);

        return base.AddRangeAsync(list);
    }

    public override Task<IReadOnlyList<GroupType>> AddRangeAsync(IEnumerable<GroupType> entities, string actor)
    {
        var list = entities.ToList();
        foreach (var entity in list)
            NormalizationHelper.ApplyNormalization(entity);

        return base.AddRangeAsync(list, actor);
    }

    // Add any custom queries or methods for GroupType here if needed
}
