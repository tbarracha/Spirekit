using SpireApi.Application.Modules.Iam.Domain.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Shared.Operations.Attributes;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupModerationOperations;

public class GetModerationAuditsByMemberIdDto
{
    public Guid GroupMemberId { get; set; }
}

[OperationGroup("IAM Group Moderation")]
[OperationRoute("moderation/audit/by-member")]
public class GetModerationAuditsByMemberIdOperation
    : BaseGroupDomainOperation<GetModerationAuditsByMemberIdDto, IReadOnlyList<GroupMemberAudit>>
{
    public GetModerationAuditsByMemberIdOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<IReadOnlyList<GroupMemberAudit>> ExecuteAsync(AuditableRequestDto<GetModerationAuditsByMemberIdDto> request)
    {
        return await _groupContext.RepositoryContext.GroupMemberAuditRepository
            .GetAuditsByMemberIdAsync(request.Data.GroupMemberId);
    }
}
