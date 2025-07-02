using System.ComponentModel;

namespace {Namespace}.Contracts.Dtos.Modules.ProjectInitializer;

public class ProjectInitializerRequestDto
{
    [DefaultValue("{Namespace}")]
    public required string Namespace { get; set; }

    [DefaultValue("web-api")]
    public required string ProjectType { get; set; } // e.g. "web-api", "blazor-app"

    public required List<ModuleSelectionDto> Modules { get; set; } = new();
}

public class ModuleSelectionDto
{
    [DefaultValue("Auth")]
    public required string Name { get; set; }    // e.g. "Auth", "Iam"

    [DefaultValue(true)]
    public required bool Enabled { get; set; }

    // Optional: You can add settings per module if needed in future
    // public Dictionary<string, string>? Settings { get; set; }
}

