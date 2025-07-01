using SpireCLI.Commands.Projects.DotNet;
using SpireCLI.Commands.Root;
using SpireCore.Commands;

namespace SpireCLI.Commands;

public static class CommandManagerBuilder
{
    public static CommandManager BuildCommandManager()
    {
        var root = new CommandNode();

        // Top-level commands
        root.AddSubNode(new CommandNode(new HelloCommand()));
        root.AddSubNode(new CommandNode(new HelpCommand()));
        root.AddSubNode(new CommandNode(new VersionCommand()));

        // DotNet command group
        var dotnetGroup = new CommandNode("dotnet", "Commands for .NET projects");
        dotnetGroup.AddSubNode(new CommandNode(new CreateDotNetApiProjectCommand()));
        root.AddSubNode(dotnetGroup);

        return new CommandManager(root);
    }
}
