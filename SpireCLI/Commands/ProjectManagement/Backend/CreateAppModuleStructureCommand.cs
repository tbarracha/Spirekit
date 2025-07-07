using System.Xml.Linq;
using SpireCLI.Commands.ProjectManagement.Core;
using SpireCore.Commands;

namespace SpireCLI.Commands.ProjectManagement.Backend;

public class CreateAppModuleStructureCommand : BaseSpireProjectCommand
{
    public override string Name => "create-app-structure";
    public override string Description =>
        "Creates a new Application Module or Feature (aliases: create-app-feature, create-app-module)";
    public override IEnumerable<string> Aliases =>
        new[] { "create-app-feature", "create-app-module" };

    public override CommandResult Execute(CommandContext context)
    {
        var calledAs = context.InvokedCommandName?.ToLowerInvariant() ?? Name;
        if (calledAs == "create-app-structure" || context.Args.Length < 2)
        {
            return CommandResult.Error(
                "Usage:\n" +
                "  spire project create-app-feature <FeatureName> <DomainModel1,DomainModel2,...>\n" +
                "  spire project create-app-module  <ModuleName>  <DomainModel1,DomainModel2,...>");
        }

        string folderType = calledAs.EndsWith("feature") ? "Features"
                           : calledAs.EndsWith("module") ? "Modules"
                           : throw new InvalidOperationException();

        var itemName = context.Args[0];
        var domainModels = context.Args[1]
                           .Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim())
                           .Where(s => s.Length > 0)
                           .ToList();

        var config = SpireProjectConfigManager.LoadConfig()
                     ?? throw new InvalidOperationException("Run 'spire set-solution' first – no .spire.config.json.");

        var parentDir = folderType == "Modules"
                        ? config.ApplicationModules?.Path
                        : config.ApplicationFeatures?.Path;
        if (string.IsNullOrWhiteSpace(parentDir) || !Directory.Exists(parentDir))
            return CommandResult.Error($"Cannot locate '{folderType}' directory – run 'spire set-solution' again?");

        /* ---- scaffolding ---- */

        var rootDir = Path.Combine(parentDir, itemName);
        var allDirs = CreateModuleOrFeatureStructureFoldersOnly(rootDir, domainModels);
        var tempFiles = CreateTempFiles(allDirs);                       // drop __temp.cs

        /* ---- project file handling ---- */

        var appProject = config.Projects?.FirstOrDefault(
                             p => p.Name.EndsWith(".Application", StringComparison.OrdinalIgnoreCase))
                         ?? throw new InvalidOperationException("Could not resolve SpireApi.Application project.");
        var appProjDir = Path.GetDirectoryName(appProject.Path)!;

        var extensionsFile = CreateModuleOrFeatureExtensionsFile(appProjDir, folderType, itemName, rootDir);
        CreateDomainModelFiles(appProjDir, folderType, itemName, rootDir, domainModels);

        var relativeRootDir = Path.GetRelativePath(appProjDir, rootDir);
        AddFolderToCsproj(appProject.Path, relativeRootDir);                         // wildcard include
        AddFileToCsproj(appProject.Path, Path.GetRelativePath(appProjDir, extensionsFile));

        RemoveTempFilesWithDelay(tempFiles);                                         // wait 500 ms → delete

        /* ---- update .spire.config.json ---- */

        if (folderType == "Modules" && config.ApplicationModules != null &&
            !config.ApplicationModules.Modules.Any(m => m.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)))
        {
            config.ApplicationModules.Modules.Add(new SpireProjectConfigManager.SpireSolutionConfig.ModuleEntry
            {
                Name = itemName,
                Path = rootDir
            });
        }
        if (folderType == "Features" && config.ApplicationFeatures != null &&
            !config.ApplicationFeatures.Features.Any(f => f.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)))
        {
            config.ApplicationFeatures.Features.Add(new SpireProjectConfigManager.SpireSolutionConfig.FeatureEntry
            {
                Name = itemName,
                Path = rootDir
            });
        }
        SpireProjectConfigManager.SaveConfig(config);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{folderType.TrimEnd('s')} '{itemName}' scaffolding created under {rootDir}");
        Console.ResetColor();
        return CommandResult.Success("");
    }

    /* -------------------------------------------------------------------------
       FOLDER & TEMP-FILE CREATION
       ---------------------------------------------------------------------- */

    private static List<string> CreateModuleOrFeatureStructureFoldersOnly(
    string rootDir,
    List<string> domainModels)
    {
        var created = new List<string>();
        void Mk(string path) { Directory.CreateDirectory(path); created.Add(path); }

        // ── top-level scaffold
        Mk(rootDir);
        Mk(Path.Combine(rootDir, "Configuration"));
        Mk(Path.Combine(rootDir, "Domain"));
        Mk(Path.Combine(rootDir, "Domain", "Services"));    // shared services
        Mk(Path.Combine(rootDir, "EventHandling"));
        Mk(Path.Combine(rootDir, "Infrastructure"));
        Mk(Path.Combine(rootDir, "Operations"));            // root operations folder

        // ── one aggregate folder per <DomainModel>
        foreach (var model in domainModels)
        {
            var plural = Pluralize(model);
            var aggregate = Path.Combine(rootDir, "Domain", plural);

            Mk(aggregate);
            Mk(Path.Combine(aggregate, "Contexts"));
            Mk(Path.Combine(aggregate, "Models"));
            Mk(Path.Combine(aggregate, "Repositories"));
            Mk(Path.Combine(aggregate, "Dtos"));

            // operations stay at root: /Operations/<Aggregate>
            Mk(Path.Combine(rootDir, "Operations", plural));
        }

        return created;
    }

    private static List<string> CreateTempFiles(IEnumerable<string> dirs)
    {
        var files = new List<string>();
        foreach (var dir in dirs)
        {
            var f = Path.Combine(dir, "__temp.cs");
            if (!File.Exists(f))
            {
                File.WriteAllText(f, "// temp placeholder – will be removed automatically");
                files.Add(f);
            }
        }
        return files;
    }

    /// <summary>Waits ~500 ms then deletes the temp placeholders.</summary>
    private static void RemoveTempFilesWithDelay(IEnumerable<string> files)
    {
        Thread.Sleep(500);                               // allow IDE / build to pick them up
        foreach (var f in files)
        {
            try { File.Delete(f); }
            catch { /* ignore */ }
        }
    }

    /* -------------------------------------------------------------------------
       FILE GENERATION (extensions + domain entities)
       ---------------------------------------------------------------------- */

    private static string CreateModuleOrFeatureExtensionsFile(
        string appProjDir, string folderType, string itemName, string rootDir)
    {
        var fileType = folderType == "Modules" ? "Module" : "Feature";
        var fileName = $"{itemName}{fileType}Extensions.cs";
        var ns = GetAppProjectNamespace(appProjDir, folderType, itemName);

        var content = $@"using Microsoft.Extensions.DependencyInjection;

namespace {ns};

public static class {itemName}{fileType}Extensions
{{
    public static IServiceCollection Add{itemName}{fileType}Services(this IServiceCollection services)
    {{
        // register services here
        return services;
    }}
}}
";
        var filePath = Path.Combine(rootDir, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    private static void CreateDomainModelFiles(
    string appProjDir,
    string folderType,
    string itemName,
    string rootDir,
    List<string> domainModels)
    {
        var baseNs = GetAppProjectNamespace(appProjDir, folderType, itemName);
        var infraDir = Path.Combine(rootDir, "Infrastructure");
        var baseEntityFile = Path.Combine(infraDir, $"Base{itemName}Entity.cs");

        // ---- shared base entity
        if (!File.Exists(baseEntityFile))
        {
            Directory.CreateDirectory(infraDir);
            File.WriteAllText(baseEntityFile,
    $@"namespace {baseNs}.Infrastructure;

public abstract class Base{itemName}Entity : BaseAuditableEntity<Guid>
{{
}}
");
        }

        // ---- per-aggregate concrete entities
        foreach (var model in domainModels)
        {
            var plural = Pluralize(model);
            var modelsDir = Path.Combine(rootDir, "Domain", plural, "Models");
            Directory.CreateDirectory(modelsDir);

            var modelFile = Path.Combine(modelsDir, $"{model}.cs");
            if (File.Exists(modelFile)) continue;

            File.WriteAllText(modelFile,
    $@"using {baseNs}.Infrastructure;

namespace {baseNs}.Domain.{plural}.Models;

public class {model} : Base{itemName}Entity
{{
    public string Name {{ get; set; }} = default!;
    public string? Description {{ get; set; }}
}}
");
        }
    }



    /* -------------------------------------------------------------------------
       .csproj helpers
       ---------------------------------------------------------------------- */

    private static void AddFolderToCsproj(string csprojPath, string relativeFolderPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;
        var ig = doc.Root.Elements(ns + "ItemGroup").FirstOrDefault() ?? new XElement(ns + "ItemGroup");

        if (!doc.Descendants(ns + "Compile")
                .Any(e => e.Attribute("Include")?.Value.StartsWith(relativeFolderPath, StringComparison.OrdinalIgnoreCase) == true))
        {
            ig.Add(new XElement(ns + "Compile", new XAttribute("Include", $@"{relativeFolderPath}\**\*.cs")));
            if (!doc.Root.Elements(ns + "ItemGroup").Any()) doc.Root.Add(ig);
            doc.Save(csprojPath);
        }
    }

    private static void AddFileToCsproj(string csprojPath, string relativeFilePath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root!.Name.Namespace;

        if (!doc.Descendants(ns + "Compile")
                .Any(e => string.Equals(e.Attribute("Include")?.Value, relativeFilePath, StringComparison.OrdinalIgnoreCase)))
        {
            var ig = doc.Root.Elements(ns + "ItemGroup").FirstOrDefault() ?? new XElement(ns + "ItemGroup");
            ig.Add(new XElement(ns + "Compile", new XAttribute("Include", relativeFilePath)));
            if (!doc.Root.Elements(ns + "ItemGroup").Any()) doc.Root.Add(ig);
            doc.Save(csprojPath);
        }
    }

    /* --------------------------------------------------------------------- */

    private static string GetAppProjectNamespace(string appProjDir, string folderType, string itemName) =>
        $"{new DirectoryInfo(appProjDir).Name}.{folderType}.{itemName}";

    private static string Pluralize(string name) =>
        name.EndsWith("y", StringComparison.OrdinalIgnoreCase) && name.Length > 1 ? name[..^1] + "ies"
      : name.EndsWith('s') ? name + "es"
      : name + "s";
}
