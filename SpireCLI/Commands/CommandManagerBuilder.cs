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

        return new CommandManager(root);
    }
}
