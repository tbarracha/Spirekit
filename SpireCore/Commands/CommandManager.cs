namespace SpireCore.Commands;

/// <summary>
/// The CommandManager is responsible for orchestrating CLI execution.
/// It parses arguments, resolves and dispatches commands, handles
/// help and interactive modes, and prints command listings.
/// </summary>
public class CommandManager
{
    // CLI metadata fields (name, version, description)
    private readonly string _cliName;
    private readonly string _version;
    private readonly string _description;
    private readonly CommandNode _root; // The root of the command tree

    public CommandNode Root => _root;

    /// <summary>
    /// Constructs a CommandManager for the specified command tree and metadata.
    /// </summary>
    public CommandManager(
        CommandNode root,
        string cliName = "SpireCLI",
        string version = "v0.1.0",
        string description = "A project scaffolder for .NET + Angular")
    {
        _root = root;
        _cliName = cliName;
        _version = version;
        _description = description;
    }

    /// <summary>
    /// Main entrypoint. Parses args and runs appropriate mode (help, interactive, command).
    /// </summary>
    public int Run(string[] args)
    {
        Console.WriteLine("\n");

        // Show help if no arguments are given.
        if (args.Length == 0)
        {
            PrintWelcome(true);
            PrintAvailableCommands(_root, true);
            return 0;
        }

        // Launch interactive mode if requested
        if (args.Contains("--interactive"))
            return RunInteractive();

        // Otherwise, run one-shot mode
        return RunOneShot(args);
    }

    /// <summary>
    /// Runs a command immediately and returns its result code.
    /// </summary>
    public int RunOneShot(string[] args)
    {
        var context = new CommandContext(args, this, _root);
        var result = RunCommand(context);
        Console.WriteLine("\n");
        return result;
    }

    /// <summary>
    /// Runs the CLI in interactive mode (REPL).
    /// </summary>
    public int RunInteractive()
    {
        HandleHelp(); // Show help at session start

        while (true)
        {
            Console.Write("\n> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Contains("--help") || args.Contains("-h"))
            {
                HandleHelp();
                continue;
            }

            var context = new CommandContext(args, this, _root, isInteractive: true);
            RunCommand(context);
            Console.WriteLine("\n");
        }

        return 0;
    }

    /// <summary>
    /// Handles showing the available commands (used for help).
    /// </summary>
    private int HandleHelp()
    {
        PrintAvailableCommands(_root);
        return 0;
    }

    /// <summary>
    /// Resolves and executes the requested command, handling errors and unknowns.
    /// </summary>
    private int RunCommand(CommandContext context)
    {
        var node = _root;
        var remaining = context.Args.ToList();

        while (remaining.Any())
        {
            var next = remaining[0];
            if (!node.TryGetSubNode(next, out var child))
                break;

            node = child;
            remaining.RemoveAt(0);
        }

        if (node.Command is null)
        {
            var identifier = string.IsNullOrWhiteSpace(node.Name)
                ? node.Description
                : node.Name;

            Console.Error.WriteLine($"[ERROR] Unknown command or missing action under '{identifier}'.");
            Console.WriteLine();
            PrintAvailableCommands(_root);
            return 1;
        }

        var commandContext = new CommandContext(remaining.ToArray(), this, _root, context.IsInteractive);
        return node.Command.Execute(commandContext);
    }

    /// <summary>
    /// Pretty prints the list of available commands, including aliases, with aligned descriptions.
    /// </summary>
    public void PrintAvailableCommands(CommandNode root, bool newLineAtEnd = false)
    {
        // Collect all commands for alignment
        var commands = new List<(string Indent, string GroupKey, string Prefix, string Name, string Description, IEnumerable<string> Aliases)>();
        CollectCommands(root, "", isRootLevel: true, groupKey: "", output: commands);

        // Determine max command length for alignment
        int maxCommandLength = commands
            .Select(c =>
            {
                var fullName = c.Name;
                if (c.Aliases.Any())
                    fullName += $" ({string.Join(", ", c.Aliases)})";
                return (c.Indent + c.Prefix + (string.IsNullOrWhiteSpace(c.Prefix) ? "" : " ") + fullName).Length;
            })
            .Max();

        // Print header
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Available commands:\n");
        Console.ResetColor();

        string lastGroup = null;

        foreach (var cmd in commands)
        {
            string groupKey = cmd.GroupKey;

            // Spacing and group header
            if (groupKey != lastGroup)
            {
                if (lastGroup != null)
                    Console.WriteLine();

                if (!string.IsNullOrWhiteSpace(groupKey))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  {groupKey}:");
                    Console.ResetColor();
                }

                lastGroup = groupKey;
            }

            // Adjust indent: root-level (no group) gets 2 spaces like group headers
            string indent = string.IsNullOrWhiteSpace(groupKey) ? "  " : "    ";

            Console.Write(indent);

            if (!string.IsNullOrWhiteSpace(cmd.Prefix))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(cmd.Prefix + " ");
            }

            var fullName = cmd.Name;
            if (cmd.Aliases.Any())
                fullName += $" ({string.Join(", ", cmd.Aliases)})";

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(fullName);

            int currentLength = (indent + cmd.Prefix + (string.IsNullOrWhiteSpace(cmd.Prefix) ? "" : " ") + fullName).Length;
            Console.Write(new string(' ', maxCommandLength - currentLength));

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" : {cmd.Description}");

            Console.ResetColor();
        }

        if (newLineAtEnd)
            Console.WriteLine();
    }

    /// <summary>
    /// Recursively collects all commands from the tree for pretty-printing.
    /// </summary>
    private void CollectCommands(
        CommandNode node,
        string prefix,
        bool isRootLevel,
        string groupKey,
        List<(string Indent, string GroupKey, string Prefix, string Name, string Description, IEnumerable<string> Aliases)> output)
    {
        string indent = isRootLevel ? "  " : "    ";

        if (node.Command is not null)
        {
            string effectiveGroupKey = string.IsNullOrWhiteSpace(groupKey) ? "" : groupKey;
            output.Add((indent, effectiveGroupKey, prefix, node.Name, node.Description, node.Command.Aliases));
        }

        if (node.SubNodes.Any())
        {
            string newGroupKey = string.IsNullOrWhiteSpace(prefix) ? node.Name : $"{prefix} {node.Name}";
            foreach (var child in node.SubNodes.OrderBy(n => n.Name))
            {
                string newPrefix = string.IsNullOrWhiteSpace(prefix) ? node.Name : $"{prefix} {node.Name}";
                CollectCommands(child, newPrefix, isRootLevel: false, groupKey: newGroupKey, output);
            }
        }
    }

    /// <summary>
    /// Prints a stylized CLI banner (welcome message).
    /// </summary>
    public void PrintWelcome(bool newLineAtEnd = false)
    {
        var border = new string('═', 50);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"╔{border}╗");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"║{_cliName.PadLeft((_cliName.Length + 50) / 2).PadRight(50)}║");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"║{$"Version {_version}".PadLeft(32).PadRight(50)}║");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"║{" ".PadRight(50)}║");

        if (!string.IsNullOrWhiteSpace(_description))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"║{_description.PadLeft((_description.Length + 50) / 2).PadRight(50)}║");
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"╚{border}╝");

        Console.ResetColor();

        if (newLineAtEnd)
            Console.WriteLine();
    }
}
