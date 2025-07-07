using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupTypeOperations;

public class UpdateGroupTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

[OperationGroup("IAM Group Types")]
[OperationRoute("group-type/update")]
public class UpdateGroupTypeOperation
    : BaseGroupDomainOperation<UpdateGroupTypeDto, GroupType?>
{
    public UpdateGroupTypeOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupType?> ExecuteAsync(AuditableRequestDto<UpdateGroupTypeDto> request)
    {
        var dto = request.Data;
        var entity = await _groupContext.RepositoryContext.GroupTypeRepository.GetByIdAsync(dto.Id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        await _groupContext.RepositoryContext.GroupTypeRepository.UpdateAsync(entity);
        return entity;
    }
}
