namespace SpireCore.Commands;

/// <summary>
/// BaseCommand is a convenience abstract base for all commands,
/// providing default implementations for Name, Description, and Aliases.
/// </summary>
public abstract class BaseCommand : ICommand
{
    /// <summary>
    /// Command name defaults to the class name minus "Command", lowercased.
    /// </summary>
    public virtual string Name => GetType().Name.Replace("Command", "").ToLowerInvariant();

    /// <summary>
    /// Default description (override in derived).
    /// </summary>
    public virtual string Description => "No description provided.";

    /// <summary>
    /// Default: no aliases (override in derived).
    /// </summary>
    public virtual IEnumerable<string> Aliases => Enumerable.Empty<string>();

    /// <summary>
    /// Command logic (must be implemented in derived).
    /// </summary>
    public abstract int Execute(CommandContext context);

    /// <summary>
    /// Optional utility for printing commands for internal custom commands.
    /// </summary>
    protected void PrintAvailableCommands(CommandNode node, string prefix = "")
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Available commands:\n");

        foreach (var child in node.SubNodes.OrderBy(n => n.Name))
        {
            if (child.Command is not null)
            {
                var fullCommand = $"{prefix}{child.Name}";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(fullCommand.PadRight(15));

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($": {child.Description}");
            }

            if (child.SubNodes.Any())
                PrintAvailableCommands(child, prefix + child.Name + " ");
        }

        Console.ResetColor();
    }
}
