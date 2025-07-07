using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupModerationOperations;

public class GetModerationAuditsByGroupIdDto
{
    public Guid GroupId { get; set; }
}

[OperationGroup("IAM Group Moderation")]
[OperationRoute("moderation/audit/by-group")]
public class GetModerationAuditsByGroupIdOperation
    : BaseGroupDomainOperation<GetModerationAuditsByGroupIdDto, IReadOnlyList<GroupMemberAudit>>
{
    public GetModerationAuditsByGroupIdOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<IReadOnlyList<GroupMemberAudit>> ExecuteAsync(AuditableRequestDto<GetModerationAuditsByGroupIdDto> request)
    {
        return await _groupContext.RepositoryContext.GroupMemberAuditRepository
            .GetAuditsByGroupIdAsync(request.Data.GroupId);
    }
}
