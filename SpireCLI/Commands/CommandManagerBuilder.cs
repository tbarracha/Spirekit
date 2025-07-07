using SpireCLI.Commands.Root;
using SpireCLI.Commands.Root.AI;
using SpireCLI.Commands.Root.ProjectManagement;
using SpireCLI.Commands.Root.ProjectManagement.Backend;
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


        // Spire Projects
        var spireProjectsSubNode = new CommandNode("project", "Projects Command Group");
        root.AddSubNode(spireProjectsSubNode);

        spireProjectsSubNode.AddSubNode(new CommandNode(new CreateNewSpireApiProject()));
        spireProjectsSubNode.AddSubNode(new CommandNode(new SetActiveSolutionCommand()));
        spireProjectsSubNode.AddSubNode(new CommandNode(new CreateAppModuleStructureCommand()));
        //spireProjectsSubNode.AddSubNode(new CommandNode(new DetectSpireApiProjectCommand()));

        return new CommandManager(root);
    }
}
