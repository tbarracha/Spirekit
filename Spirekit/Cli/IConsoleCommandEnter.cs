namespace Spirekit.Cli;

public interface IConsoleCommandEnter
{
    Task OnConsoleCommandEnterAsync(IServiceProvider services);
}
