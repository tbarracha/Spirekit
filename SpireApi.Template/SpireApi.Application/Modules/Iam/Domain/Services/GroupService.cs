using SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;

namespace SpireApi.Application.Modules.Iam.Domain.Services;

public class GroupService
{
    private readonly GroupRepository _groupRepository;
    private readonly GroupTypeRepository _groupTypeRepository;
    private readonly GroupMemberRepository _groupMemberRepository;

    public GroupService(
        GroupRepository groupRepository,
        GroupTypeRepository groupTypeRepository,
        GroupMemberRepository groupMemberRepository
    )
    {
        _groupRepository = groupRepository;
        _groupTypeRepository = groupTypeRepository;
        _groupMemberRepository = groupMemberRepository;
    }
}
