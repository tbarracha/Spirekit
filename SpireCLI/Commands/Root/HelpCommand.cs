
using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

public class HelpCommand : BaseCommand
{
    public override string Name => "help";
    public override IEnumerable<string> Aliases => new[] { "h", "-h", "--help" };
    public override string Description => "Show help information";

    public override int Execute(CommandContext context)
    {
        var cmdManager = context.CommandManager;
        cmdManager.PrintWelcome(true);
        cmdManager.PrintAvailableCommands(cmdManager.Root);

        return 0;
    }
}
