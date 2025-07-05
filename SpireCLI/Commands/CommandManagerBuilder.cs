using SpireCLI.Commands.Root;
using SpireCLI.Commands.Root.AI;
using SpireCLI.Commands.Root.AI.ChatGpt;
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

        // AI commands
        var aiSubNode = new CommandNode("ai", "AI Command Group");
        root.AddSubNode(aiSubNode);

        aiSubNode.AddSubNode(new CommandNode(new OllamaChatCommand()));
        aiSubNode.AddSubNode(new CommandNode(new ChatGptBrowserChatCommand()));

        return new CommandManager(root);
    }
}
