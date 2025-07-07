using System.Text.Json;

namespace SpireCLI.Commands.ProjectManagement.Core;

/// <summary>
/// Handles .spire.config.json loading, saving, and related operations.
/// </summary>
public static class SpireProjectConfigManager
{
    // --- File Resolution ---
    public static string? FindConfigFilePath()
    {
        var cwd = Directory.GetCurrentDirectory();
        var gitRoot = FindGitRoot(cwd);
        if (gitRoot != null)
            return Path.Combine(gitRoot, ".spire.config.json");

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var userConfig = Path.Combine(home, ".spire.config.json");
        return File.Exists(userConfig) ? userConfig : null;
    }

    public static string? FindGitRoot(string startDir)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, ".git")))
                return dir.FullName;
            dir = dir.Parent;
        }
        return null;
    }

    // --- Load and Save Config ---
    public static SpireSolutionConfig? LoadConfig()
    {
        var configPath = FindConfigFilePath();
        if (configPath == null || !File.Exists(configPath))
            return null;
        try
        {
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<SpireSolutionConfig>(json);
        }
        catch { return null; }
    }

    public static void SaveConfig(SpireSolutionConfig config)
    {
        var configPath = FindConfigFilePath();
        if (configPath == null)
            throw new InvalidOperationException("Could not determine config file path.");

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configPath, json);
    }

    public static string? GetActiveSolution()
    {
        var config = LoadConfig();
        return config?.SolutionPath;
    }

    // --- Utility methods for scanning Modules and Features ---

    public static SpireSolutionConfig.ModuleSection? FindModulesSection(string appRoot)
    {
        var modulesDir = Path.Combine(appRoot, "Modules");
        if (Directory.Exists(modulesDir))
        {
            var modules = Directory.GetDirectories(modulesDir)
                .Select(modDir => new SpireSolutionConfig.ModuleEntry
                {
                    Name = Path.GetFileName(modDir),
                    Path = modDir
                }).ToList();

            return new SpireSolutionConfig.ModuleSection
            {
                Path = modulesDir,
                Modules = modules
            };
        }
        return null;
    }

    public static SpireSolutionConfig.FeatureSection? FindFeaturesSection(string appRoot)
    {
        var featuresDir = Path.Combine(appRoot, "Features");
        if (Directory.Exists(featuresDir))
        {
            var features = Directory.GetDirectories(featuresDir)
                .Select(featDir => new SpireSolutionConfig.FeatureEntry
                {
                    Name = Path.GetFileName(featDir),
                    Path = featDir
                }).ToList();

            return new SpireSolutionConfig.FeatureSection
            {
                Path = featuresDir,
                Features = features
            };
        }
        return null;
    }

    // --- SpireSolutionConfig STRUCT (same as before, but move to shared location if not already) ---
    public class SpireSolutionConfig
    {
        public string SolutionPath { get; set; }
        public List<ProjectEntry> Projects { get; set; }
        public ModuleSection? ApplicationModules { get; set; }
        public FeatureSection? ApplicationFeatures { get; set; }

        public class ProjectEntry
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string? Template { get; set; }
        }

        public class ModuleSection
        {
            public string Path { get; set; }
            public List<ModuleEntry> Modules { get; set; }
        }
        public class ModuleEntry
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class FeatureSection
        {
            public string Path { get; set; }
            public List<FeatureEntry> Features { get; set; }
        }
        public class FeatureEntry
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}
