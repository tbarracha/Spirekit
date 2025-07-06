
using SpireApi.Application.Modules.Iam.Repositories;
using SpireCore.Services;

namespace SpireApi.Application.Modules.Iam.Domain.Services;

public class GroupService : ITransientService
{
    private readonly GroupRepository _groupRepository;
    private readonly GroupTypeRepository _groupTypeRepository;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly GroupMembershipStateRepository _groupMembershipStateRepository;
    private readonly GroupMemberAuditRepository _groupMemberAuditRepository;

    public GroupService(
        GroupRepository groupRepository,
        GroupTypeRepository groupTypeRepository,
        GroupMemberRepository groupMemberRepository,
        GroupMembershipStateRepository groupMembershipStateRepository,
        GroupMemberAuditRepository groupMemberAuditRepository)
    {
        _groupRepository = groupRepository;
        _groupTypeRepository = groupTypeRepository;
        _groupMemberRepository = groupMemberRepository;
        _groupMembershipStateRepository = groupMembershipStateRepository;
        _groupMemberAuditRepository = groupMemberAuditRepository;
    }
}
