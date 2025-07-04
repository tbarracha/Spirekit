
using SpireApi.Application.Modules.Iam.Infrastructure;

namespace SpireApi.Application.Modules.Iam.Domain.Models.Permissions;

public class PermissionScope : BaseIamEntity
{
    public string Name { get; set; } = default!; // e.g., "Project", "Document", "Invoice"
    public string? Description { get; set; }
    public List<Permission> Permissions { get; set; } = new();
}