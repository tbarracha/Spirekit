// File: Application.Modules.Iam.Domain.Models.Groups.GroupAccount.cs
using SpireApi.Application.Shared.Entities;
using SpireApi.Application.Modules.Iam.Domain.Models.Accounts;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Groups;

public class GroupAccount : GuidEntity
{
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = default!;
}
