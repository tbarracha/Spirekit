
namespace SpireApi.Application.Modules.ProjectInitializer.Domain.Models;

public class ProjectInitializerRequest
{
    public required string Namespace { get; set; }
    public required string ProjectType { get; set; }
    public required List<ModuleSelection> Modules { get; set; } = new();
}

public class ModuleSelection
{
    public required string Name { get; set; }
    public required bool Enabled { get; set; }
}
