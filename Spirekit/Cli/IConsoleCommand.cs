namespace Spirekit.Cli;

public interface IConsoleCommand
{
    int Index { get; }
    string Title { get; }
    string Description { get; }
    IConsoleCommand? Parent => null;

    Task HandleConsoleCommandAsync(IServiceProvider services);
}
