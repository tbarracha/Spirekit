using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class CreateGroupTypeDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

[OperationGroup("IAM Group Types")]
[OperationRoute("group-type/create")]
public class CreateGroupTypeOperation
    : BaseGroupDomainOperation<CreateGroupTypeDto, GroupType>
{
    public CreateGroupTypeOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupType> ExecuteAsync(AuditableRequestDto<CreateGroupTypeDto> request)
    {
        var dto = request.Data;
        var entity = new GroupType
        {
            Name = dto.Name,
            Description = dto.Description
        };
        await _groupContext.RepositoryContext.GroupTypeRepository.AddAsync(entity);
        return entity;
    }
}
