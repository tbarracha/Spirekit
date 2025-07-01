using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

/// <summary>
/// Prints a friendly greeting.
/// </summary>
internal class HelloCommand : BaseCommand
{
    public override string Name => "hello";
    public override string Description => "Prints a friendly greeting.";

    public override CommandResult Execute(CommandContext context)
    {
        Console.WriteLine("Hello, world!");
        return CommandResult.Success();
    }
}
