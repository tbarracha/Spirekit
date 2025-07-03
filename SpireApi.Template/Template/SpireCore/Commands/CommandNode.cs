namespace SpireCore.Commands;

/// <summary>
/// CommandNode represents a node in the command tree, either a command
/// (with an ICommand) or a grouping/category node (with sub-nodes).
/// It handles lookup for both direct names and aliases.
/// </summary>
public class CommandNode
{
    public string Name { get; }
    public string Description { get; }
    public ICommand Command { get; }

    // Subnodes keyed by name
    private readonly Dictionary<string, CommandNode> _subNodes = new();
    // Subnodes keyed by alias
    private readonly Dictionary<string, CommandNode> _aliases = new();

    /// <summary>
    /// Root node constructor.
    /// </summary>
    public CommandNode() : this("", "Root command node") { }

    /// <summary>
    /// Constructor for leaf command nodes.
    /// </summary>
    public CommandNode(ICommand command)
    {
        Name = command.Name;
        Description = command.Description;
        Command = command;
    }

    /// <summary>
    /// Constructor for group/category nodes.
    /// </summary>
    public CommandNode(string name, string description = "")
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Registers a subnode and its aliases.
    /// </summary>
    public void AddSubNode(CommandNode node)
    {
        _subNodes[node.Name] = node;

        // Register aliases if this is a leaf node with a command
        if (node.Command != null)
        {
            foreach (var alias in node.Command.Aliases)
            {
                if (!_aliases.ContainsKey(alias))
                    _aliases[alias] = node;
            }
        }
    }

    /// <summary>
    /// Looks up a subnode by name or alias.
    /// </summary>
    public bool TryGetSubNode(string name, out CommandNode node)
    {
        if (_subNodes.TryGetValue(name, out node))
            return true;

        if (_aliases.TryGetValue(name, out node))
            return true;

        return false;
    }

    /// <summary>
    /// Enumerates direct child nodes.
    /// </summary>
    public IEnumerable<CommandNode> SubNodes => _subNodes.Values;
}

