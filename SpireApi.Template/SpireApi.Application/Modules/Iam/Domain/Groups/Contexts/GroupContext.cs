using SpireApi.Application.Modules.Iam.Domain.Groups.Services;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;

public class GroupContext : ITransientService
{
    public GroupRepositoryContext RepositoryContext { get; }
    public GroupService GroupService { get; }

    public GroupContext(
        GroupRepositoryContext repositoryContext,
        GroupService groupService)
    {
        RepositoryContext = repositoryContext;
        GroupService = groupService;
    }
}