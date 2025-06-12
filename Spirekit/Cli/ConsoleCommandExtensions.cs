namespace Spirekit.Cli;

public static class ConsoleCommandExtensions
{
    public static List<IConsoleCommand> GetSubCommands(
        this IConsoleCommand parent,
        IEnumerable<IConsoleCommand> all) =>
        all.Where(c => c.Parent == parent)
           .OrderBy(c => c.Index)
           .ToList();

    public static List<IConsoleCommand> GetSubCommandsFor(
        this IEnumerable<IConsoleCommand> allCommands,
        IConsoleCommand parent)
    {
        return allCommands
            .Where(c => c.Parent == parent)
            .OrderBy(c => c.Index)
            .ToList();
    }
}