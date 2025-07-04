// --------- BaseGroupMemberCrudOperation.cs ---------
using SpireApi.Application.Modules.Iam.Domain.Models.Groups;
using SpireApi.Application.Modules.Iam.Infrastructure;
using SpireApi.Shared.Operations.Dtos;

namespace SpireApi.Application.Modules.Iam.Operations.Groups.GroupMemberOperations;

/// <summary>
/// Abstract base class for all Group Member CRUD operations.
/// </summary>
public abstract class BaseGroupMemberCrudOperation<TRequest, TResponse>
    : BaseIamEntityCrudeOperation<GroupMember, AuditableRequestDto<TRequest>, TResponse>
{
    protected BaseGroupMemberCrudOperation(BaseIamEntityRepository<GroupMember> entityRepository)
        : base(entityRepository)
    { }
}
