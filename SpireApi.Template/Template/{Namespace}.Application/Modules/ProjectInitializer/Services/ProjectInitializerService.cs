using System.IO.Compression;
using {Namespace}.Application.Modules.ProjectInitializer.Domain.Models;
using SpireCore.API.Services;

namespace {Namespace}.Application.Modules.ProjectInitializer.Services;

public class ProjectInitializerService : IProjectInitializerService, ITransientService
{
    private const string BaseTemplatesRoot = "{Namespace}.Template/Template"; // Always relative to project root!

    public async Task<InitializedProject> InitializeProjectAsync(ProjectInitializerRequest request)
    {
        // 1. Prepare workspace
        var tempFolder = Path.Combine(Path.GetTempPath(), $"project_init_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempFolder);

        // 2. Check if a {Namespace}.sln exists in BaseTemplatesRoot
        string templateRoot = Path.GetFullPath(BaseTemplatesRoot, AppContext.BaseDirectory);
        string namespacePlaceholder = "{Namespace}";
        string solutionPattern = $"{namespacePlaceholder}.sln";
        string solutionFile = Directory.GetFiles(templateRoot, solutionPattern, SearchOption.TopDirectoryOnly).FirstOrDefault();

        if (solutionFile == null)
            throw new FileNotFoundException($"Template solution '{solutionPattern}' not found in {templateRoot}");

        // 3. Copy template root (everything) to temp
        CopyDirectory(templateRoot, tempFolder);

        // 4. Replace all {Namespace} in all file/folder names and all contents
        ReplaceNamespaceInDirectory(tempFolder, namespacePlaceholder, request.Namespace);

        // 5. Prune modules (if needed)
        PruneModules(tempFolder, request.Namespace, request.Modules?.Where(m => m.Enabled).Select(m => m.Name).ToList());

        // 6. Zip project
        var zipPath = Path.Combine(Path.GetTempPath(), $"{request.Namespace}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip");
        ZipFile.CreateFromDirectory(tempFolder, zipPath);
        var zipBytes = await File.ReadAllBytesAsync(zipPath);

        // (Optional) Clean up files after reading
        Directory.Delete(tempFolder, recursive: true);
        File.Delete(zipPath);

        // 7. Return result
        return new InitializedProject
        {
            ProjectType = request.ProjectType,
            FileName = Path.GetFileName(zipPath),
            ZipFile = zipBytes,
            TempFolderPath = tempFolder,
            IncludedModules = request.Modules?.Where(m => m.Enabled).Select(m => m.Name).ToList() ?? new List<string>()
        };
    }

    // Recursively copy all files and subdirs
    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), overwrite: true);

        foreach (var subdir in Directory.GetDirectories(sourceDir))
            CopyDirectory(subdir, Path.Combine(targetDir, Path.GetFileName(subdir)));
    }

    /// <summary>
    /// Recursively replace all folder/file names and file contents with the new namespace.
    /// </summary>
    private static void ReplaceNamespaceInDirectory(string rootDir, string token, string replacement)
    {
        // 1. Rename directories and files
        foreach (var dir in Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
        {
            var dirName = Path.GetFileName(dir);
            if (dirName.Contains(token))
            {
                var newDir = Path.Combine(Path.GetDirectoryName(dir)!, dirName.Replace(token, replacement));
                Directory.Move(dir, newDir);
            }
        }

        foreach (var file in Directory.GetFiles(rootDir, "*", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(file);
            if (fileName.Contains(token))
            {
                var newFile = Path.Combine(Path.GetDirectoryName(file)!, fileName.Replace(token, replacement));
                File.Move(file, newFile);
            }
        }

        // 2. Replace in all file contents (.cs, .csproj, .sln, .json, .xml, .md, etc.)
        var patterns = new[] { "*.cs", "*.csproj", "*.sln", "*.json", "*.xml", "*.md" };
        foreach (var pattern in patterns)
        {
            foreach (var file in Directory.GetFiles(rootDir, pattern, SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(file);
                if (text.Contains(token))
                {
                    File.WriteAllText(file, text.Replace(token, replacement));
                }
            }
        }
    }

    /// <summary>
    /// Remove any modules that are not in the includedModules list, if a Modules folder exists.
    /// </summary>
    private static void PruneModules(string rootDir, string namespaceName, List<string>? includedModules)
    {
        if (includedModules == null || includedModules.Count == 0)
            return;

        // Find {Namespace}.Application/Modules
        var appFolder = Directory.GetDirectories(rootDir, $"{namespaceName}.Application", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (appFolder != null)
        {
            var modulesPath = Path.Combine(appFolder, "Modules");
            if (Directory.Exists(modulesPath))
            {
                foreach (var dir in Directory.GetDirectories(modulesPath))
                {
                    var moduleName = Path.GetFileName(dir);
                    if (!includedModules.Contains(moduleName, StringComparer.OrdinalIgnoreCase))
                    {
                        Directory.Delete(dir, recursive: true);
                    }
                }
            }
        }
    }
}

