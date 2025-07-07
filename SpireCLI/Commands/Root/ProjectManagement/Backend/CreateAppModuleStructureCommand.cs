using System.Xml.Linq;
using SpireCLI.Commands.Root.ProjectManagement.Core;
using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

public class CreateAppModuleStructureCommand : BaseSpireProjectCommand
{
    public override string Name => "create-app-structure";
    public override string Description => "Creates a new Application Module or Feature (use: create-app-feature, create-app-module)";
    public override IEnumerable<string> Aliases => new[] { "create-app-feature", "create-app-module" };

    public override CommandResult Execute(CommandContext context)
    {
        var calledAs = context.InvokedCommandName?.ToLowerInvariant() ?? Name.ToLowerInvariant();
        if (calledAs == "create-app-structure" || context.Args.Length < 2)
        {
            return CommandResult.Error(
                "Usage:\n" +
                "  spire project create-app-feature <FeatureName> <DomainModel1,DomainModel2,...>\n" +
                "  spire project create-app-module <ModuleName> <DomainModel1,DomainModel2,...>\n" +
                "Example:\n" +
                "  spire project create-app-feature Hello Message\n" +
                "  spire project create-app-module Product Product,Category"
            );
        }

        string folderType;
        if (calledAs.EndsWith("feature"))
            folderType = "Features";
        else if (calledAs.EndsWith("module"))
            folderType = "Modules";
        else
            return CommandResult.Error("Unknown type. Use create-app-feature or create-app-module.");

        var itemName = context.Args[0];
        var domainModels = context.Args[1].Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        var config = SpireProjectConfigManager.LoadConfig();
        if (config == null)
            return CommandResult.Error("No .spire.config.json found. Please run 'spire set-solution' first.");

        string? parentDir = null;
        if (folderType == "Modules")
            parentDir = config.ApplicationModules?.Path;
        else if (folderType == "Features")
            parentDir = config.ApplicationFeatures?.Path;

        if (string.IsNullOrWhiteSpace(parentDir) || !Directory.Exists(parentDir))
            return CommandResult.Error($"Could not locate '{folderType}' folder in your application. Check your .spire.config.json or run 'spire set-solution'.");

        var rootDir = Path.Combine(parentDir, itemName);
        CreateModuleOrFeatureStructureFoldersOnly(rootDir, domainModels);

        var appProject = config.Projects?.FirstOrDefault(p => p.Name.EndsWith(".Application", StringComparison.OrdinalIgnoreCase));
        if (appProject == null || !File.Exists(appProject.Path))
            return CommandResult.Error("Could not find your .Application .csproj file from config. Please check .spire.config.json.");

        string appProjectFolder = Path.GetDirectoryName(appProject.Path)!;

        // Create the extensions file and include it in the project
        var extensionsFile = CreateModuleOrFeatureExtensionsFile(
            appProjectFolder, folderType, itemName, rootDir);
        var relativeRootDir = Path.GetRelativePath(appProjectFolder, rootDir);
        AddFolderToCsproj(appProject.Path, relativeRootDir);
        var relativeExtensionsFile = Path.GetRelativePath(appProjectFolder, extensionsFile);
        AddFileToCsproj(appProject.Path, relativeExtensionsFile);

        // Update config's ApplicationModules/Features section
        if (folderType == "Modules" && config.ApplicationModules != null)
        {
            if (!config.ApplicationModules.Modules.Any(m => m.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)))
            {
                config.ApplicationModules.Modules.Add(new SpireProjectConfigManager.SpireSolutionConfig.ModuleEntry
                {
                    Name = itemName,
                    Path = rootDir
                });
            }
        }
        else if (folderType == "Features" && config.ApplicationFeatures != null)
        {
            if (!config.ApplicationFeatures.Features.Any(f => f.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)))
            {
                config.ApplicationFeatures.Features.Add(new SpireProjectConfigManager.SpireSolutionConfig.FeatureEntry
                {
                    Name = itemName,
                    Path = rootDir
                });
            }
        }
        SpireProjectConfigManager.SaveConfig(config);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{folderType.TrimEnd('s')} '{itemName}' folder structure created and added to project under {rootDir}");
        Console.ResetColor();

        return CommandResult.Success("");
    }

    private static void CreateModuleOrFeatureStructureFoldersOnly(string rootDir, List<string> domainModels)
    {
        Directory.CreateDirectory(rootDir);
        Directory.CreateDirectory(Path.Combine(rootDir, "Configuration"));
        Directory.CreateDirectory(Path.Combine(rootDir, "Domain", "Contexts"));

        var modelsDir = Path.Combine(rootDir, "Domain", "Models");
        Directory.CreateDirectory(modelsDir);
        foreach (var model in domainModels)
            Directory.CreateDirectory(Path.Combine(modelsDir, Pluralize(model)));

        var reposDir = Path.Combine(rootDir, "Domain", "Repositories");
        Directory.CreateDirectory(reposDir);
        foreach (var model in domainModels)
            Directory.CreateDirectory(Path.Combine(reposDir, Pluralize(model)));

        Directory.CreateDirectory(Path.Combine(rootDir, "Domain", "Services"));

        var dtosDir = Path.Combine(rootDir, "Dtos");
        Directory.CreateDirectory(dtosDir);
        foreach (var model in domainModels)
            Directory.CreateDirectory(Path.Combine(dtosDir, $"{model}Dtos"));

        Directory.CreateDirectory(Path.Combine(rootDir, "EventHandling"));
        Directory.CreateDirectory(Path.Combine(rootDir, "Infrastructure"));

        var opsDir = Path.Combine(rootDir, "Operations");
        Directory.CreateDirectory(opsDir);
        foreach (var model in domainModels)
            Directory.CreateDirectory(Path.Combine(opsDir, Pluralize(model)));
    }

    private static string CreateModuleOrFeatureExtensionsFile(string appProjectFolder, string folderType, string itemName, string rootDir)
    {
        string fileType = folderType == "Modules" ? "Module" : "Feature";
        string fileName = $"{itemName}{fileType}Extensions.cs";
        string ns = GetAppProjectNamespace(appProjectFolder, folderType, itemName);

        string boilerplate = $@"using Microsoft.Extensions.DependencyInjection;

namespace {ns};

public static class {itemName}{fileType}Extensions
{{
    public static IServiceCollection Add{itemName}{fileType}Services(this IServiceCollection services)
    {{
        return services;
    }}
}}
";
        string filePath = Path.Combine(rootDir, fileName);
        File.WriteAllText(filePath, boilerplate);
        return filePath;
    }

    private static string GetAppProjectNamespace(string appProjectFolder, string folderType, string itemName)
    {
        // App project folder name becomes project namespace, e.g. SpireApi.Application
        string appProjectName = new DirectoryInfo(appProjectFolder).Name;
        return $"{appProjectName}.{folderType}.{itemName}";
    }

    private static void AddFolderToCsproj(string csprojPath, string relativeFolderPath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root?.Name.Namespace ?? "";

        var itemGroup = doc.Root!.Elements(ns + "ItemGroup").FirstOrDefault()
                     ?? new XElement(ns + "ItemGroup");

        bool alreadyIncluded = doc.Descendants(ns + "Compile")
            .Any(e => e.Attribute("Include")?.Value.StartsWith(relativeFolderPath, StringComparison.OrdinalIgnoreCase) == true);

        if (!alreadyIncluded)
        {
            var compileElem = new XElement(ns + "Compile");
            compileElem.SetAttributeValue("Include", relativeFolderPath + @"\**\*.cs");
            itemGroup.Add(compileElem);

            if (!doc.Root.Elements(ns + "ItemGroup").Any())
                doc.Root.Add(itemGroup);

            doc.Save(csprojPath);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Added <Compile Include=\"{relativeFolderPath}\\**\\*.cs\" /> to {csprojPath}");
            Console.ResetColor();
        }
    }

    private static void AddFileToCsproj(string csprojPath, string relativeFilePath)
    {
        var doc = XDocument.Load(csprojPath);
        var ns = doc.Root?.Name.Namespace ?? "";

        bool alreadyIncluded = doc.Descendants(ns + "Compile")
            .Any(e => string.Equals(e.Attribute("Include")?.Value, relativeFilePath, StringComparison.OrdinalIgnoreCase));

        if (!alreadyIncluded)
        {
            var itemGroup = doc.Root!.Elements(ns + "ItemGroup").FirstOrDefault()
                         ?? new XElement(ns + "ItemGroup");
            var compileElem = new XElement(ns + "Compile");
            compileElem.SetAttributeValue("Include", relativeFilePath);
            itemGroup.Add(compileElem);

            if (!doc.Root.Elements(ns + "ItemGroup").Any())
                doc.Root.Add(itemGroup);

            doc.Save(csprojPath);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Added <Compile Include=\"{relativeFilePath}\" /> to {csprojPath}");
            Console.ResetColor();
        }
    }

    private static string Pluralize(string name)
    {
        if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase) && name.Length > 1)
            return name.Substring(0, name.Length - 1) + "ies";
        if (name.EndsWith("s"))
            return name + "es";
        return name + "s";
    }
}
