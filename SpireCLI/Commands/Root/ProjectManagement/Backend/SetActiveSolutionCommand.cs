using SpireCLI.Commands.Root.ProjectManagement.Core;
using SpireCore.Commands;
using System.Text.Json;
using static SpireCLI.Commands.Root.ProjectManagement.Core.SpireProjectConfigManager;
using static SpireCLI.Commands.Root.ProjectManagement.Core.SpireProjectConfigManager.SpireSolutionConfig; // For SpireSolutionConfig

namespace SpireCLI.Commands.Root;

public class SetActiveSolutionCommand : BaseSpireProjectCommand
{
    public override string Name => "set-solution";
    public override string Description => "Sets the path to the main Spire solution (.sln) this CLI should operate on.";
    public override IEnumerable<string> Aliases => new[] { "solution", "select-solution", "config-solution" };

    public override CommandResult Execute(CommandContext context)
    {
        if (context.Args.Length == 0)
            return CommandResult.Error("Please provide the path to the Spire solution file (e.g., Spirekit.sln) or a folder containing a .sln file.");

        var inputPath = context.Args[0].Trim('"');
        var solutionPath = ResolveSolutionPath(inputPath);
        if (solutionPath == null)
            return CommandResult.Error("Provided path does not exist or is not a .sln file or folder.");

        var projectEntries = ParseSolutionProjects(solutionPath);
        AssignProjectTemplates(projectEntries);
        WarnIfMissingCrucialProjects(projectEntries);

        // === Find modules/features in .Application ===
        var applicationProject = projectEntries
            .FirstOrDefault(p => p.Name.EndsWith(".Application", StringComparison.OrdinalIgnoreCase));
        ModuleSection? modulesSection = null;
        FeatureSection? featuresSection = null;
        if (applicationProject != null)
        {
            var appRoot = Path.GetDirectoryName(applicationProject.Path)!;
            modulesSection = SpireProjectConfigManager.FindModulesSection(appRoot);
            featuresSection = SpireProjectConfigManager.FindFeaturesSection(appRoot);
        }

        // Save config via the utility class
        var config = new SpireSolutionConfig
        {
            SolutionPath = solutionPath,
            Projects = projectEntries,
            ApplicationModules = modulesSection,
            ApplicationFeatures = featuresSection
        };
        SpireProjectConfigManager.SaveConfig(config);

        PrintSummary(solutionPath, projectEntries, modulesSection, featuresSection);

        return CommandResult.Success("");
    }

    // -- Helpers --

    private static string? ResolveSolutionPath(string inputPath)
    {
        if (File.Exists(inputPath) && inputPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            return Path.GetFullPath(inputPath);

        if (Directory.Exists(inputPath))
        {
            var slnFiles = Directory.GetFiles(inputPath, "*.sln", SearchOption.TopDirectoryOnly);
            if (slnFiles.Length == 0)
                return null;
            if (slnFiles.Length == 1)
                return Path.GetFullPath(slnFiles[0]);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Multiple solution files found:");
            for (int i = 0; i < slnFiles.Length; i++)
                Console.WriteLine($"  [{i + 1}] {Path.GetFileName(slnFiles[i])}");
            Console.ResetColor();

            Console.Write("Enter the number of the solution you want to use: ");
            var selection = Console.ReadLine();
            if (int.TryParse(selection, out int index) && index >= 1 && index <= slnFiles.Length)
                return Path.GetFullPath(slnFiles[index - 1]);
        }
        return null;
    }

    private static List<SpireSolutionConfig.ProjectEntry> ParseSolutionProjects(string solutionPath)
    {
        var solutionDir = Path.GetDirectoryName(solutionPath)!;
        var slnLines = File.ReadAllLines(solutionPath);
        var entries = new List<SpireSolutionConfig.ProjectEntry>();

        foreach (var line in slnLines)
        {
            if (line.TrimStart().StartsWith("Project("))
            {
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    var name = parts[0].Split('=')[1].Trim().Trim('"');
                    var relPath = parts[1].Trim().Trim('"');
                    var absProjPath = Path.GetFullPath(Path.Combine(solutionDir, relPath));
                    entries.Add(new SpireSolutionConfig.ProjectEntry
                    {
                        Name = name,
                        Path = absProjPath
                    });
                }
            }
        }
        return entries;
    }

    private static void AssignProjectTemplates(List<SpireSolutionConfig.ProjectEntry> projects)
    {
        foreach (var proj in projects)
            proj.Template = DetectProjectTemplate(proj.Name, proj.Path);
    }

    private static void WarnIfMissingCrucialProjects(List<SpireSolutionConfig.ProjectEntry> projects)
    {
        var requiredSuffixes = new[]
        {
            (Suffix: ".Application",    Template: "classlib"),
            (Suffix: ".Contracts",      Template: "classlib"),
            (Suffix: ".Host",           Template: "webapi"),
            (Suffix: ".Infrastructure", Template: "classlib"),
            (Suffix: "SpireCore",       Template: "classlib"),
            (Suffix: "SpireCLI",        Template: "cli"),
        };

        foreach (var (suffix, _) in requiredSuffixes)
        {
            if (!projects.Any(p => p.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️  Warning: Project with suffix '{suffix}' not found in this solution.");
                Console.ResetColor();
            }
        }
    }

    private static string? DetectProjectTemplate(string projectName, string projectPath)
    {
        if (projectName.Equals("SpireCLI", StringComparison.OrdinalIgnoreCase))
            return "cli";
        if (!File.Exists(projectPath) || !projectPath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            return null;
        try
        {
            var firstLines = File.ReadLines(projectPath).Take(5).ToList();
            foreach (var line in firstLines)
            {
                if (line.Contains("<Project Sdk=\"Microsoft.NET.Sdk.Web\""))
                    return "webapi";
                if (line.Contains("<Project Sdk=\"Microsoft.NET.Sdk\""))
                    return "classlib";
            }
        }
        catch { /* ignore */ }
        return null;
    }

    private static void PrintSummary(
        string solutionPath,
        List<SpireSolutionConfig.ProjectEntry> projects,
        SpireSolutionConfig.ModuleSection? modulesSection,
        SpireSolutionConfig.FeatureSection? featuresSection)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Solution set to: {solutionPath}");
        Console.WriteLine($"Found projects:");
        foreach (var entry in projects)
            Console.WriteLine($"  - {entry.Name} ({entry.Path}){(entry.Template != null ? $" [{entry.Template}]" : "")}");
        if (modulesSection != null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Modules detected in .Application:");
            foreach (var mod in modulesSection.Modules)
                Console.WriteLine($"  • {mod.Name} [{mod.Path}]");
        }
        if (featuresSection != null)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Features detected in .Application:");
            foreach (var feat in featuresSection.Features)
                Console.WriteLine($"  • {feat.Name} [{feat.Path}]");
        }
        Console.ResetColor();

        // Config file location
        var configFile = SpireProjectConfigManager.FindConfigFilePath();
        if (!string.IsNullOrWhiteSpace(configFile))
            Console.WriteLine($"Configuration saved at {configFile}");
    }
}
