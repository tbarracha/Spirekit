using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

public class CreateAppStructureCommand : BaseCommand
{
    public override string Name => "create-app-structure";
    public override string Description => "Creates a new Application Module or Feature (use: create-app-feature, create-app-module)";
    public override IEnumerable<string> Aliases => new[] { "create-app-feature", "create-app-module" };

    public override CommandResult Execute(CommandContext context)
    {
        // Detect if this is being called as 'create-app-feature' or 'create-app-module'
        var calledAs = context.InvokedCommandName?.ToLowerInvariant() ?? Name.ToLowerInvariant();

        // Print help if called as 'create-app-structure' directly
        if (calledAs == "create-app-structure" || context.Args.Length < 3)
        {
            return CommandResult.Error(
                "Usage:\n" +
                "  spire project create-app-feature <FeatureName> <AppRootPath> <DomainModel1,DomainModel2,...>\n" +
                "  spire project create-app-module <ModuleName> <AppRootPath> <DomainModel1,DomainModel2,...>\n" +
                "Example:\n" +
                "  spire project create-app-feature Hello C:\\Path\\To\\SpireApi.Application Message\n" +
                "  spire project create-app-module Product C:\\Path\\To\\SpireApi.Application Product,Category"
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
        var appRoot = context.Args[1];
        var domainModels = context.Args[2].Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        if (!Directory.Exists(appRoot))
            return CommandResult.Error($"Application root path does not exist: {appRoot}");

        var rootDir = Path.Combine(appRoot, folderType, itemName);
        CreateModuleOrFeatureStructure(rootDir, itemName, domainModels);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{folderType.TrimEnd('s')} '{itemName}' structure created under {rootDir}");
        Console.ResetColor();

        return CommandResult.Success("");
    }

    private static void CreateModuleOrFeatureStructure(string rootDir, string moduleName, List<string> domainModels)
    {
        Directory.CreateDirectory(rootDir);

        // 1. Configuration
        var configDir = Path.Combine(rootDir, "Configuration");
        Directory.CreateDirectory(configDir);
        File.WriteAllText(Path.Combine(configDir, $"{moduleName}Configuration.cs"), "");

        // 2. Domain/Contexts
        var contextsDir = Path.Combine(rootDir, "Domain", "Contexts");
        Directory.CreateDirectory(contextsDir);
        File.WriteAllText(Path.Combine(contextsDir, $"{moduleName}Context.cs"), "");
        File.WriteAllText(Path.Combine(contextsDir, $"{moduleName}RepositoryContext.cs"), "");

        // 3. Domain/Models/{ModelName}s
        var modelsDir = Path.Combine(rootDir, "Domain", "Models");
        Directory.CreateDirectory(modelsDir);
        foreach (var model in domainModels)
        {
            var plural = Pluralize(model);
            var modelSubdir = Path.Combine(modelsDir, plural);
            Directory.CreateDirectory(modelSubdir);
            File.WriteAllText(Path.Combine(modelSubdir, $"{model}.cs"), "");
        }

        // 4. Domain/Repositories/{ModelName}s
        var reposDir = Path.Combine(rootDir, "Domain", "Repositories");
        Directory.CreateDirectory(reposDir);
        foreach (var model in domainModels)
        {
            var plural = Pluralize(model);
            var repoSubdir = Path.Combine(reposDir, plural);
            Directory.CreateDirectory(repoSubdir);
            File.WriteAllText(Path.Combine(repoSubdir, $"I{model}Repository.cs"), "");
            File.WriteAllText(Path.Combine(repoSubdir, $"{model}Repository.cs"), "");
        }

        // 5. Domain/Services
        var servicesDir = Path.Combine(rootDir, "Domain", "Services");
        Directory.CreateDirectory(servicesDir);
        foreach (var model in domainModels)
        {
            File.WriteAllText(Path.Combine(servicesDir, $"{model}Service.cs"), "");
        }

        // 6. Dtos/{ModelName}Dtos
        var dtosDir = Path.Combine(rootDir, "Dtos");
        Directory.CreateDirectory(dtosDir);
        foreach (var model in domainModels)
        {
            var dtoSubdir = Path.Combine(dtosDir, $"{model}Dtos");
            Directory.CreateDirectory(dtoSubdir);
            File.WriteAllText(Path.Combine(dtoSubdir, $"{model}CreateDto.cs"), "");
            File.WriteAllText(Path.Combine(dtoSubdir, $"{model}UpdateDto.cs"), "");
            File.WriteAllText(Path.Combine(dtoSubdir, $"{model}ReadDto.cs"), "");
        }

        // 7. EventHandling
        Directory.CreateDirectory(Path.Combine(rootDir, "EventHandling"));

        // 8. Infrastructure
        var infraDir = Path.Combine(rootDir, "Infrastructure");
        Directory.CreateDirectory(infraDir);
        File.WriteAllText(Path.Combine(infraDir, $"Base{moduleName}DbContext.cs"), "");
        File.WriteAllText(Path.Combine(infraDir, $"Base{moduleName}Entity.cs"), "");
        File.WriteAllText(Path.Combine(infraDir, $"Base{moduleName}EntityRepository.cs"), "");

        // 9. Operations/{ModelName}s
        var opsDir = Path.Combine(rootDir, "Operations");
        Directory.CreateDirectory(opsDir);
        foreach (var model in domainModels)
        {
            var plural = Pluralize(model);
            var opSubdir = Path.Combine(opsDir, plural);
            Directory.CreateDirectory(opSubdir);
            File.WriteAllText(Path.Combine(opSubdir, $"Get{model}Operation.cs"), "");
            File.WriteAllText(Path.Combine(opSubdir, $"Create{model}Operation.cs"), "");
            File.WriteAllText(Path.Combine(opSubdir, $"Update{model}Operation.cs"), "");
            File.WriteAllText(Path.Combine(opSubdir, $"Delete{model}Operation.cs"), "");
        }

        // 10. {ModuleName}Extensions.cs and {ModuleName}ReadMe.md
        File.WriteAllText(Path.Combine(rootDir, $"{moduleName}Extensions.cs"), "");
        File.WriteAllText(Path.Combine(rootDir, $"{moduleName}ReadMe.md"), "");
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
