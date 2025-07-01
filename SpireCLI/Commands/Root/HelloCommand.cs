
using SpireCore.Commands;

namespace SpireCLI.Commands.Root;

internal class HelloCommand : BaseCommand
{
    public override string Name => "hello";

    public override string Description => "Prints a friendly greeting.";

    public override int Execute(CommandContext context)
    {
        Console.WriteLine("Hello, world!");
        return 0;
    }
}
