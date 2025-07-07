using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Roles.Models;

public class Role : BaseIamEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
