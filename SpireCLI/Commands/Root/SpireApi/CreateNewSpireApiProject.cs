using SpireCLI.Commands.Root.SpireApi.Core;
using SpireCore.Commands;
using System.Diagnostics;

namespace SpireCLI.Commands.Root.SpireApi;

/// <summary>
/// Creates a new SpireApi solution with full multi-project layout.
/// </summary>
public class CreateNewSpireApiProject : BaseSpireApiCommand
{
    public override string Name => "new";
    public override string Description => "Create a new SpireApi solution with recommended project structure.";
    public override IEnumerable<string> Aliases => new[] { "create", "init" };

    public override CommandResult Execute(CommandContext context)
    {
        if (context.Args.Length == 0)
            return CommandResult.Error("Please specify a solution/project name. Example: spireapi new MySolution");

        var solutionName = context.Args[0];
        var targetDir = Path.Combine(Directory.GetCurrentDirectory(), solutionName);

        if (Directory.Exists(targetDir))
            return CommandResult.Error($"Directory '{solutionName}' already exists. Please choose a different name or delete the existing folder.");

        try
        {
            Directory.CreateDirectory(targetDir);
            Directory.SetCurrentDirectory(targetDir);

            // 1. Create the solution
            RunProcess("dotnet", $"new sln -n {solutionName}");

            // 2. Project definitions: name, template
            var projects = new[]
            {
                    (Name: $"{solutionName}.Application",      Template: "classlib"),
                    (Name: $"{solutionName}.Contracts",        Template: "classlib"),
                    (Name: $"{solutionName}.Host",             Template: "webapi"),    // For minimal API, still use webapi and remove WeatherForecast
                    (Name: $"{solutionName}.Infrastructure",   Template: "classlib"),
                    (Name: $"{solutionName}.SpireCore.API",    Template: "classlib"),
                    (Name: $"{solutionName}.SpireCore",        Template: "classlib")
                };

            // 3. Create projects
            foreach (var (projName, template) in projects)
            {
                RunProcess("dotnet", $"new {template} -n {projName}");

                // Remove WeatherForecast and Controllers if it's Host
                if (projName.EndsWith(".Host", StringComparison.OrdinalIgnoreCase))
                {
                    var weather = Path.Combine(projName, "WeatherForecast.cs");
                    if (File.Exists(weather)) File.Delete(weather);
                    var controllersDir = Path.Combine(projName, "Controllers");
                    if (Directory.Exists(controllersDir)) Directory.Delete(controllersDir, true);

                    // Overwrite Program.cs with minimal API template
                    var programPath = Path.Combine(projName, "Program.cs");
                    File.WriteAllText(programPath, $@"var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet(""/"", () => ""Hello from {solutionName} Minimal API!"");

app.Run();");
                }
            }

            // 4. Add all projects to solution
            foreach (var (projName, _) in projects)
            {
                RunProcess("dotnet", $"sln add {Path.Combine(projName, $"{projName}.csproj")}");
            }

            // 5. Optionally, add references (example: Application references Contracts, Infrastructure, SpireCore)
            // This can be extended based on actual dependency graph
            RunProcess("dotnet", $"add {projects[0].Name}/{projects[0].Name}.csproj reference {projects[1].Name}/{projects[1].Name}.csproj");
            RunProcess("dotnet", $"add {projects[0].Name}/{projects[0].Name}.csproj reference {projects[3].Name}/{projects[3].Name}.csproj");
            RunProcess("dotnet", $"add {projects[0].Name}/{projects[0].Name}.csproj reference {projects[5].Name}/{projects[5].Name}.csproj");

            // Host references Application and Contracts
            RunProcess("dotnet", $"add {projects[2].Name}/{projects[2].Name}.csproj reference {projects[0].Name}/{projects[0].Name}.csproj");
            RunProcess("dotnet", $"add {projects[2].Name}/{projects[2].Name}.csproj reference {projects[1].Name}/{projects[1].Name}.csproj");

            // 6. Write solution-level README.md
            File.WriteAllText("README.md", $@"# {solutionName} – SpireApi Solution

Created by SpireCLI.

## Projects

- `{solutionName}.Application`      : Application services, orchestrators
- `{solutionName}.Contracts`        : DTOs, interfaces, external contracts
- `{solutionName}.Host`             : ASP.NET Minimal API host
- `{solutionName}.Infrastructure`   : DbContexts, repositories, migrations
- `{solutionName}.SpireCore.API`    : API core utilities
- `{solutionName}.SpireCore`        : Core business logic, entities, etc.

## Getting Started

    cd {solutionName}
    dotnet restore
    dotnet build
    dotnet run --project {solutionName}.Host/{solutionName}.Host.csproj
");

            // 7. .gitignore
            File.WriteAllText(".gitignore", @"bin/
obj/
.vscode/
.idea/
*.user
*.suo
*.DotSettings.user
*.db
");

            return CommandResult.Success(
                $"\n✔ SpireApi solution '{solutionName}' scaffolded with 6 projects and solution file!\n\n" +
                $"Next steps:\n  cd {solutionName}\n  dotnet restore\n  dotnet build\n  dotnet run --project {solutionName}.Host/{solutionName}.Host.csproj\n"
            );
        }
        catch (Exception ex)
        {
            return CommandResult.Error($"Failed to create solution: {ex.Message}");
        }
    }

    // Helper: runs a shell process and throws on failure
    private void RunProcess(string command, string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var proc = Process.Start(psi)!;
        proc.WaitForExit();
        if (proc.ExitCode != 0)
            throw new Exception($"`{command} {args}` failed:\n{proc.StandardError.ReadToEnd()}");
    }
}