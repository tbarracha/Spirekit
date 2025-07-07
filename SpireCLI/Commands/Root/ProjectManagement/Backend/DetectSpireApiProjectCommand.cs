using SpireCLI.Commands.Root.ProjectManagement.Core;
using SpireCore.Commands;

namespace SpireCLI.Commands.Root.ProjectManagement.Backend;

public class DetectSpireApiProjectCommand : BaseSpireProjectCommand
{
    public override string Name => "detect";
    public override string Description => "Detects whether the current folder is a SpireApi solution.";
    public override IEnumerable<string> Aliases => new[] { "is-spireapi", "verify" };

    public override CommandResult Execute(CommandContext context)
    {
        // Allow passing a folder, default to CWD
        var targetDir = context.Args.Length > 0
            ? Path.GetFullPath(context.Args[0])
            : Directory.GetCurrentDirectory();

        if (!Directory.Exists(targetDir))
            return CommandResult.Error($"Directory '{targetDir}' does not exist.");

        // Look for a .sln file in the root
        var slnFiles = Directory.GetFiles(targetDir, "*.sln");
        if (slnFiles.Length == 0)
            return CommandResult.Error("No .sln (solution) file found. This does not appear to be a .NET solution.");

        // Use the solution name as the project prefix
        var solutionName = Path.GetFileNameWithoutExtension(slnFiles[0]);

        // The expected project names (not case sensitive)
        var requiredProjects = new[]
        {
            $"{solutionName}.Application",
            $"{solutionName}.Contracts",
            $"{solutionName}.Host",
            $"{solutionName}.Infrastructure",
            $"{solutionName}.SpireCore.API",
            $"{solutionName}.SpireCore"
        };

        // Count how many expected projects exist
        int found = requiredProjects.Count(name => Directory.Exists(Path.Combine(targetDir, name)));

        // Optionally, check for key files in those folders (e.g., .csproj, Program.cs)
        int csprojsFound = requiredProjects.Count(name =>
        {
            var folder = Path.Combine(targetDir, name);
            return Directory.Exists(folder) &&
                Directory.GetFiles(folder, "*.csproj").Any();
        });

        // Print a detection result with "confidence"
        if (found >= 4 && csprojsFound >= 4)
        {
            return CommandResult.Success(
                $"✅ This folder **appears to be a SpireApi solution!**\n" +
                $"Found .sln: {Path.GetFileName(slnFiles[0])}\n" +
                $"Found {found}/6 required project folders ({csprojsFound} with .csproj files).\n");
        }
        else if (found >= 2)
        {
            return CommandResult.Success(
                $"⚠️ Partial SpireApi project detected.\n" +
                $"Found .sln: {Path.GetFileName(slnFiles[0])}\n" +
                $"Only {found}/6 required project folders detected.\n" +
                $"Consider running `spireapi new {solutionName}` to re-scaffold or add missing projects."
            );
        }
        else
        {
            return CommandResult.Error(
                $"❌ This folder does **not** look like a SpireApi project. \n" +
                $"Only {found}/6 required project folders detected. If this is incorrect, check the folder names."
            );
        }
    }
}
