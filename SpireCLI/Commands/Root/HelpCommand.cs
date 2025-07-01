using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

/// <summary>
/// Shows help information.
/// </summary>
public class HelpCommand : BaseCommand
{
    public override string Name => "help";
    public override IEnumerable<string> Aliases => new[] { "h", "-h", "--help" };
    public override string Description => "Show help information";

    public override CommandResult Execute(CommandContext context)
    {
        var cmdManager = context.CommandManager;
        cmdManager.PrintWelcome(true);
        cmdManager.PrintAvailableCommands(cmdManager.Root);
        return CommandResult.Success();
    }
}
