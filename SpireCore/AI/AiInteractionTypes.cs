namespace SpireCore.AI;

public static class AiInteractionTypes
{
    public const string Text = "text";
    public const string Image = "image";
    public const string File = "file";
    public const string Audio = "audio";

    /// <summary>
    /// List of all defined attachment types.
    /// </summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
            Text,
            Image,
            File,
            Audio,
    };
}