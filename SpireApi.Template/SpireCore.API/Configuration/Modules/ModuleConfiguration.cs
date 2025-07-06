namespace SpireCore.API.Configuration.Modules;

public class ModuleConfiguration
{
    public bool Enabled { get; set; } = true;
    public string? DbConnectionString { get; set; }

    // Support for arbitrary custom settings
    public Dictionary<string, object>? Settings { get; set; }
}