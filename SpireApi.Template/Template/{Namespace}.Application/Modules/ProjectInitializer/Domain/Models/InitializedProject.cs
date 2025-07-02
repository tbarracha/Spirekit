namespace {Namespace}.Application.Modules.ProjectInitializer.Domain.Models;

public class InitializedProject
{
    public required string ProjectType { get; set; }
    public required string FileName { get; set; }
    public required byte[] ZipFile { get; set; }

    // Optionally, you can add more info here for debugging or logging:
    public string? TempFolderPath { get; set; }      // (Optional) Path to the temp folder used
    public List<string>? IncludedModules { get; set; } // (Optional) List of included modules
}

