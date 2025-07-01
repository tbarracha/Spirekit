using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

/// <summary>
/// Shows version information.
/// </summary>
public class VersionCommand : BaseCommand
{
    public override string Name => "version";
    public override IEnumerable<string> Aliases => new[] { "v", "-v", "--version" };
    public override string Description => "Show version information";

    public override CommandResult Execute(CommandContext context)
    {
        context.CommandManager.PrintWelcome();
        return CommandResult.Success();
    }
}
