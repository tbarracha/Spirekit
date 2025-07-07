using SpireApi.Application.Modules.Iam.Domain.Groups.Repositories;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;

public class GroupRepositoryContext : ITransientService
{
    public GroupRepository GroupRepository { get; }
    public GroupTypeRepository GroupTypeRepository { get; }
    public GroupMemberRepository GroupMemberRepository { get; }
    public GroupMembershipStateRepository GroupMembershipStateRepository { get; }
    public GroupMemberAuditRepository GroupMemberAuditRepository { get; }

    public GroupRepositoryContext(
        GroupRepository groupRepository,
        GroupTypeRepository groupTypeRepository,
        GroupMemberRepository groupMemberRepository,
        GroupMembershipStateRepository groupMembershipStateRepository,
        GroupMemberAuditRepository groupMemberAuditRepository)
    {
        GroupRepository = groupRepository;
        GroupTypeRepository = groupTypeRepository;
        GroupMemberRepository = groupMemberRepository;
        GroupMembershipStateRepository = groupMembershipStateRepository;
        GroupMemberAuditRepository = groupMemberAuditRepository;
    }
}