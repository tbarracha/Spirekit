using SpireCore.Commands;
using System.Diagnostics;

namespace SpireCLI.Commands.Projects.DotNet;

/// <summary>
/// Scaffolds a new .NET API solution with Application, Contracts, and API layers.
/// </summary>
public class CreateDotNetApiProjectCommand : BaseCommand
{
    public override string Name => "new-api";
    public override string Description => "Scaffold a new .NET API solution with Application, Contracts, and API layers.";

    public override CommandResult Execute(CommandContext context)
    {
        if (context.Args.Length == 0)
        {
            var err = "[ERROR] You must provide a project name: dotnet new-api MyProject";
            Console.Error.WriteLine(err);
            return CommandResult.Error(err);
        }

        var projName = context.Args[0];
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), projName);

        if (Directory.Exists(rootPath))
        {
            var err = $"[ERROR] Directory '{projName}' already exists.";
            Console.Error.WriteLine(err);
            return CommandResult.Error(err);
        }

        Directory.CreateDirectory(rootPath);

        if (!Run("dotnet", $"new classlib -n {projName}.Application", rootPath, out var error1)) return CommandResult.Error(error1);
        if (!Run("dotnet", $"new classlib -n {projName}.Contracts", rootPath, out var error2)) return CommandResult.Error(error2);
        if (!Run("dotnet", $"new webapi -n {projName}.API", rootPath, out var error3)) return CommandResult.Error(error3);

        var apiPath = Path.Combine(rootPath, $"{projName}.API");
        var appPath = Path.Combine(rootPath, $"{projName}.Application");
        var contractsPath = Path.Combine(rootPath, $"{projName}.Contracts");

        if (!Run("dotnet", $"add {apiPath} reference {contractsPath}", rootPath, out var error4)) return CommandResult.Error(error4);
        if (!Run("dotnet", $"add {apiPath} reference {appPath}", rootPath, out var error5)) return CommandResult.Error(error5);
        if (!Run("dotnet", $"add {appPath} reference {contractsPath}", rootPath, out var error6)) return CommandResult.Error(error6);

        var successMsg = $"\n[DONE] Project '{projName}' created successfully!";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(successMsg);
        Console.ResetColor();
        return CommandResult.Success(successMsg);
    }

    /// <summary>
    /// Runs a process and returns true if successful; captures error otherwise.
    /// </summary>
    private bool Run(string file, string args, string workingDir, out string error)
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
            error = process.StandardError.ReadToEnd();
            Console.Error.WriteLine(error);
            return false;
        }
        else
        {
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            error = string.Empty;
            return true;
        }
    }
}
