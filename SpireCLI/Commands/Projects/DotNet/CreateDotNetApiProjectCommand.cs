using SpireCore.Commands;
using System.Diagnostics;

namespace SpireCLI.Commands.Projects.DotNet;

public class CreateDotNetApiProjectCommand : BaseCommand
{
    public override string Name => "new-api";
    public override string Description => "Scaffold a new .NET API solution with Application, Contracts, and API layers.";

    public override int Execute(CommandContext context)
    {
        if (context.Args.Length == 0)
        {
            Console.Error.WriteLine("[ERROR] You must provide a project name: dotnet new-api MyProject");
            return 1;
        }

        var projName = context.Args[0];
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), projName);

        if (Directory.Exists(rootPath))
        {
            Console.Error.WriteLine($"[ERROR] Directory '{projName}' already exists.");
            return 1;
        }

        Directory.CreateDirectory(rootPath);

        Run("dotnet", $"new classlib -n {projName}.Application", rootPath);
        Run("dotnet", $"new classlib -n {projName}.Contracts", rootPath);
        Run("dotnet", $"new webapi -n {projName}.API", rootPath);

        var apiPath = Path.Combine(rootPath, $"{projName}.API");
        var appPath = Path.Combine(rootPath, $"{projName}.Application");
        var contractsPath = Path.Combine(rootPath, $"{projName}.Contracts");

        Run("dotnet", $"add {apiPath} reference {contractsPath}", rootPath);
        Run("dotnet", $"add {apiPath} reference {appPath}", rootPath);
        Run("dotnet", $"add {appPath} reference {contractsPath}", rootPath);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n[DONE] Project '{projName}' created successfully!");
        Console.ResetColor();
        return 0;
    }

    private void Run(string file, string args, string workingDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = file,
            Arguments = args,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Console.Error.WriteLine(process.StandardError.ReadToEnd());
        }
        else
        {
            Console.WriteLine(process.StandardOutput.ReadToEnd());
        }
    }
}