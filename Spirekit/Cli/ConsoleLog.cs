namespace Spirekit.Cli;

public enum TitleOutline
{
    None,
    LineAbove,
    LineBelow,
    DoubleLine,
    Box
}

public enum ConsoleInputPosition
{
    Inline,
    Below
}


public static class ConsoleLog
{
    private static string Timestamp => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    public static void Info(string message) => WriteWithLevel("INFO", message, ConsoleColor.Cyan);
    public static void Warning(string message) => WriteWithLevel("WARNING", message, ConsoleColor.Yellow);
    public static void Error(string message) => WriteWithLevel("ERROR", message, ConsoleColor.Red);
    public static void Debug(string message) => WriteWithLevel("DEBUG", message, ConsoleColor.Gray);
    public static void Success(string message) => WriteWithLevel("SUCCESS", message, ConsoleColor.Green);

    public static void WriteTitle(string title, TitleOutline outline = TitleOutline.Box, ConsoleColor color = ConsoleColor.White)
    {
        var width = title.Length + 4;
        var line = new string('─', width);
        var doubleLine = new string('═', width);

        Console.ForegroundColor = color;

        switch (outline)
        {
            case TitleOutline.LineAbove:
                Console.WriteLine(line);
                Console.WriteLine($"  {title}");
                break;
            case TitleOutline.LineBelow:
                Console.WriteLine($"  {title}");
                Console.WriteLine(line);
                break;
            case TitleOutline.DoubleLine:
                Console.WriteLine(doubleLine);
                Console.WriteLine($"  {title}");
                Console.WriteLine(doubleLine);
                break;
            case TitleOutline.Box:
                Console.WriteLine($"╔{doubleLine}╗");
                Console.WriteLine($"║  {title.PadRight(width - 4)}  ║");
                Console.WriteLine($"╚{doubleLine}╝");
                break;
            case TitleOutline.None:
            default:
                Console.WriteLine($"  {title}");
                break;
        }

        Console.ResetColor();
    }

    private static void WriteWithLevel(string level, string message, ConsoleColor color)
    {
        var label = $"[{level.PadRight(7)}]";
        Write($"{label} {message}", color);
    }

    public static void Write(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"[{Timestamp}] {message}");
        Console.ResetColor();
    }

    public static void Write(string message)
    {
        Console.WriteLine($"[{Timestamp}] {message}");
    }

    public static void WriteRaw(string message, ConsoleColor? color = null)
    {
        if (color.HasValue)
            Console.ForegroundColor = color.Value;

        Console.WriteLine(message);

        if (color.HasValue)
            Console.ResetColor();
    }

    public static string ReadInput(string prompt, ConsoleInputPosition position = ConsoleInputPosition.Inline, ConsoleColor? promptColor = null)
    {
        if (promptColor.HasValue)
            Console.ForegroundColor = promptColor.Value;

        switch (position)
        {
            case ConsoleInputPosition.Inline:
                Console.Write(prompt + " ");
                break;
            case ConsoleInputPosition.Below:
                Console.WriteLine(prompt);
                break;
        }

        if (promptColor.HasValue)
            Console.ResetColor();

        return Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
