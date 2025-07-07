using SpireApi.Application.Modules.Iam.Domain.Groups.Contexts;
using SpireApi.Application.Modules.Iam.Domain.Groups.Models;
using SpireApi.Application.Modules.Iam.Operations.Groups;
using SpireCore.API.Operations.Attributes;
using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupModerationOperations;

public class GetLatestModerationAuditDto
{
    public Guid GroupMemberId { get; set; }
}

[OperationGroup("IAM Group Moderation")]
[OperationRoute("moderation/audit/latest")]
public class GetLatestModerationAuditOperation
    : BaseGroupDomainOperation<GetLatestModerationAuditDto, GroupMemberAudit?>
{
    public GetLatestModerationAuditOperation(GroupContext groupContext) : base(groupContext) { }

    public override async Task<GroupMemberAudit?> ExecuteAsync(AuditableRequestDto<GetLatestModerationAuditDto> request)
    {
        // Use the repository from the context, not direct DI
        return await _groupContext.RepositoryContext.GroupMemberAuditRepository
            .GetLatestAuditAsync(request.Data.GroupMemberId);
    }
}
