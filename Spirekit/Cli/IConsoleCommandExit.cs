namespace Spirekit.Cli;

public interface IConsoleCommandExit
{
    Task OnConsoleCommandExitAsync(IServiceProvider services);
}
