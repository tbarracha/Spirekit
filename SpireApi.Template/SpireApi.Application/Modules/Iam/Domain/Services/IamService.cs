using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Permissions.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Roles.Repositories;
using SpireApi.Application.Modules.Iam.Domain.Models.Users;
using SpireApi.Application.Modules.Iam.Domain.Models.Users.Repositories;
using SpireCore.Constants;

namespace SpireApi.Application.Modules.Iam.Domain.Services;

public class IamService
{
    private readonly IamUserRepository _iamUserRepository;

    private readonly GroupRepository _groupRepository;
    private readonly GroupTypeRepository _groupTypeRepository;
    private readonly GroupMemberRepository _groupMemberRepository;

    private readonly RoleRepository _roleRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RolePermissionRepository _rolePermissionRepository;

    private readonly PermissionRepository _permissionRepository;
    private readonly PermissionScopeRepository _permissionScopeRepository;

    public IamService(
        IamUserRepository iamUserRepository,
        GroupRepository groupRepository,
        GroupTypeRepository groupTypeRepository,
        GroupMemberRepository groupMemberRepository,
        RoleRepository roleRepository,
        UserRoleRepository userRoleRepository,
        RolePermissionRepository rolePermissionRepository,
        PermissionRepository permissionRepository,
        PermissionScopeRepository permissionScopeRepository)
    {
        _iamUserRepository = iamUserRepository;
        _groupRepository = groupRepository;
        _groupTypeRepository = groupTypeRepository;
        _groupMemberRepository = groupMemberRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _permissionScopeRepository = permissionScopeRepository;
    }

    public async Task<(IamUser user, Group group)> NewUserSetup(Guid userId)
    {
        var user = await _iamUserRepository.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        var teamGroupType = await _groupTypeRepository.GetFilteredAsync(
            gt => gt.Name.ToLower() == "team",
            state: StateFlags.ACTIVE
        );

        if (teamGroupType == null)
            throw new Exception("GroupType 'TEAM' not found");

        var group = new Group
        {
            Id = Guid.NewGuid(),
            GroupTypeId = teamGroupType.Id,
            Name = $"{user.DisplayName}'s Team",
            Description = $"Private team for {user.DisplayName}",
            StateFlag = StateFlags.ACTIVE
        };

        await _groupRepository.AddAsync(group);

        var groupMember = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = user.Id,
            StateFlag = StateFlags.ACTIVE
        };

        await _groupMemberRepository.AddAsync(groupMember);

        return (user, group);
    }
}
