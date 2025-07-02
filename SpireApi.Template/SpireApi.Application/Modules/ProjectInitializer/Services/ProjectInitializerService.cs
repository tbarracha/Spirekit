using System.IO.Compression;
using SpireApi.Application.Modules.ProjectInitializer.Domain.Models;
using SpireCore.API.Services;

namespace SpireApi.Application.Modules.ProjectInitializer.Services;

public class ProjectInitializerService : IProjectInitializerService, ITransientService
{
    // Optionally, these could be injected/configured
    private const string BaseTemplatesRoot = "ProjectTemplates"; // e.g., under your solution root
    private const string ModulesRoot = "Modules"; // e.g., relative to template or solution root

    public async Task<InitializedProject> InitializeProjectAsync(ProjectInitializerRequest request)
    {
        // 1. Prepare workspace
        var tempFolder = Path.Combine(Path.GetTempPath(), $"project_init_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempFolder);

        // 2. Copy base template
        var templateFolder = Path.Combine(BaseTemplatesRoot, request.ProjectType);
        if (!Directory.Exists(templateFolder))
            throw new DirectoryNotFoundException($"Project template '{request.ProjectType}' not found.");

        CopyDirectory(templateFolder, tempFolder);

        // 3. Add modules
        var includedModules = new List<string>();
        foreach (var module in request.Modules.Where(m => m.Enabled))
        {
            var moduleSource = Path.Combine(ModulesRoot, module.Name);
            if (Directory.Exists(moduleSource))
            {
                var moduleTarget = Path.Combine(tempFolder, "Modules", module.Name);
                CopyDirectory(moduleSource, moduleTarget);
                includedModules.Add(module.Name);
            }
            // else, optionally log or throw if missing
        }

        // 4. Replace namespace tokens (e.g., "MyCompany.Template")
        ReplaceNamespace(tempFolder, "TemplateNamespace", request.Namespace);

        // 5. Zip project
        var zipPath = Path.Combine(Path.GetTempPath(), $"{request.Namespace}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip");
        ZipFile.CreateFromDirectory(tempFolder, zipPath);
        var zipBytes = await File.ReadAllBytesAsync(zipPath);

        // (Optional) Clean up files after reading
        Directory.Delete(tempFolder, recursive: true);
        File.Delete(zipPath);

        // 6. Return result
        return new InitializedProject
        {
            ProjectType = request.ProjectType,
            FileName = Path.GetFileName(zipPath),
            ZipFile = zipBytes,
            TempFolderPath = tempFolder,
            IncludedModules = includedModules
        };
    }

    // --- Helpers ---

    // Recursively copy all files and subdirs
    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), overwrite: true);

        foreach (var subdir in Directory.GetDirectories(sourceDir))
            CopyDirectory(subdir, Path.Combine(targetDir, Path.GetFileName(subdir)));
    }

    // Recursively replace namespace tokens in text files
    private static void ReplaceNamespace(string directory, string token, string replacement)
    {
        foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
        {
            // You might want to filter for .cs, .csproj, .sln, .json, etc.
            var text = File.ReadAllText(file);
            if (text.Contains(token))
            {
                File.WriteAllText(file, text.Replace(token, replacement));
            }
        }
    }
}
