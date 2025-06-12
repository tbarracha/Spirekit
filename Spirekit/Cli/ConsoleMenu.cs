namespace Spirekit.Cli;

public static class ConsoleMenu
{
    private static readonly Dictionary<IConsoleCommand, CommandState> CommandStates = new();

    public static async Task Run(
        string title,
        IConsoleCommand? parent,
        List<IConsoleCommand> allCommands,
        IServiceProvider services)
    {
        while (true)
        {
            Console.Clear();
            ConsoleLog.WriteTitle(title, TitleOutline.Box, ConsoleColor.White);

            var commands = parent == null
                ? allCommands.Where(c => c.Parent == null).OrderBy(c => c.Index).ToList()
                : allCommands.GetSubCommandsFor(parent);

            foreach (var cmd in commands)
            {
                var state = GetCommandState(cmd).ToString().PadRight(9);
                ConsoleLog.WriteRaw($"{cmd.Index}. [{state}] {cmd.Title} — {cmd.Description}");
            }

            Console.WriteLine("\n0. Back");
            Console.Write("\nSelect option: ");
            var input = Console.ReadLine();

            if (input == "0")
                return;

            if (int.TryParse(input, out var selected))
            {
                var cmd = commands.FirstOrDefault(c => c.Index == selected);
                if (cmd != null)
                {
                    var subCommands = allCommands.GetSubCommandsFor(cmd);
                    if (subCommands.Any())
                    {
                        Run(cmd.Title, cmd, allCommands, services);
                    }
                    else
                    {
                        try
                        {
                            SetCommandState(cmd, CommandState.Starting);
                            ConsoleLog.Info($"[STARTING] Executing '{cmd.Title}'...");

                            if (cmd is IConsoleCommandEnter enterable)
                                await enterable.OnConsoleCommandEnterAsync(services);

                            SetCommandState(cmd, CommandState.Running);
                            ConsoleLog.Info($"[RUNNING ] Running logic for '{cmd.Title}'...");
                            await cmd.HandleConsoleCommandAsync(services);

                            SetCommandState(cmd, CommandState.Finished);
                            ConsoleLog.Success($"[FINISHED] Successfully completed '{cmd.Title}'.");

                            if (cmd is IConsoleCommandExit exitable)
                                await exitable.OnConsoleCommandExitAsync(services);

                            SetCommandState(cmd, CommandState.Idle);
                        }
                        catch (Exception ex)
                        {
                            SetCommandState(cmd, CommandState.Idle);
                            ConsoleLog.Error($"Command failed: {ex.Message}");
                        }

                        ConsoleLog.Info("Press any key to return to menu...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    ConsoleLog.Warning("Invalid selection.");
                    Console.ReadKey();
                }
            }
        }
    }

    private static void SetCommandState(IConsoleCommand cmd, CommandState state)
    {
        CommandStates[cmd] = state;
    }

    private static CommandState GetCommandState(IConsoleCommand cmd)
    {
        return CommandStates.TryGetValue(cmd, out var state)
            ? state
            : CommandState.Idle;
    }
}
