using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Shared.Entities;
using SpireApi.Shared.Operations;
using SpireApi.Shared.Services;

namespace SpireApi.Application.Modules.Iam.Operations.Groups;

public class GetGroupByIdOperation : CrudeOperation<Group, Guid, GuidEntityDbContext, Guid, Group?>, ITransientService
{
    public GetGroupByIdOperation(IRepository<Group, Guid> repository)
        : base(repository)
    {
    }

    public override async Task<Group?> ExecuteAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}
