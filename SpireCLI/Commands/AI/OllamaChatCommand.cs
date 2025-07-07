using SpireCore.AI.Clients;
using SpireCore.AI.Interactions.Contracts;
using SpireCore.AI.Interactions.Contracts.Attachments;
using SpireCore.AI.Interactions.Implementations;
using SpireCore.AI.Interactions.Implementations.Attachments;
using SpireCore.AI.Providers.Ollama;
using SpireCore.Commands;
using System.Text;
using System.Text.RegularExpressions;

namespace SpireCLI.Commands.AI;

internal class OllamaChatCommand : BaseCommand
{
    public override string Name => "chat-ollama";
    public override string Description => "Interactively chat with an Ollama model, keeping context (Qwen thinking supported).";

    private const string DEFAULT_MODEL = "qwen3:14b";

    public override CommandResult Execute(CommandContext context)
    {
        RunStreamingChatAsync(context).GetAwaiter().GetResult();
        return CommandResult.Success();
    }

    private async Task RunStreamingChatAsync(CommandContext context)
    {
        Console.WriteLine("== Ollama Chat ==");

        // --- Setup endpoint & model
        Console.Write("Ollama endpoint [default: http://localhost:11434]: ");
        var endpoint = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(endpoint))
            endpoint = "http://localhost:11434";

        Console.Write($"Model name [default: {DEFAULT_MODEL}]: ");
        var model = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(model))
            model = DEFAULT_MODEL;

        bool isQwen = model.ToLower().Contains("qwen");

        // --- System prompt
        Console.Write("\nSystem prompt (optional, press Enter to skip): ");
        var sysPrompt = Console.ReadLine() ?? "";

        // --- Client & session
        var aiConfig = new AiClientConfiguration
        {
            Provider = "ollama",
            Endpoint = endpoint,
            Models = new Dictionary<string, string> { ["text"] = model }
        };
        var client = new OllamaClient(new HttpClient(), aiConfig);
        var session = new InteractionSession(
            Guid.NewGuid().ToString(), aiConfig, client
        );

        if (!string.IsNullOrWhiteSpace(sysPrompt))
            session.AddUserInteraction(
                new Interaction("system", "text", new[] { new TextAttachment(sysPrompt) })
            );

        Console.WriteLine("\nType your messages below. Type /exit to quit.\n");

        while (true)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("You: ");
            Console.ResetColor();
            var userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput)) continue;
            if (userInput.Trim().Equals("/exit", StringComparison.OrdinalIgnoreCase)) break;

            // --- Qwen think toggle as flag message
            if (isQwen)
            {
                Console.Write("Thinking mode for this message? (y/N): ");
                var think = Console.ReadLine()?.Trim().ToLower();
                var toggle = think == "y" || think == "yes" ? "/think" : "/no_think";
                session.AddUserInteraction(
                    new Interaction("user", "text", new[] { new TextAttachment(toggle) })
                );
            }

            // --- Add the actual user message
            session.AddUserInteraction(
                new Interaction("user", "text", new[] { new TextAttachment(userInput) })
            );

            try
            {
                var thinkFlag = session.History.Interactions.LastOrDefault(i => i.Role == "user")?
                    .Attachments.OfType<ITextAttachment>().FirstOrDefault()?.Text?.Trim();

                var think = thinkFlag == "/think";

                var request = new InteractionRequest(
                    BuildPromptFromHistory(session),
                    stream: true,
                    think: think
                );

                // --- Begin streaming (colors exactly as you specified)
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("AI: ");
                Console.WriteLine();
                Console.ResetColor();

                var full = new StringBuilder();

                await foreach (var part in client.StreamInteractionAsync(request))
                {
                    var chunk = part
                      .Attachments
                      .OfType<ITextAttachment>()
                      .FirstOrDefault()
                      ?.Text ?? "";

                    if (string.IsNullOrEmpty(chunk))
                        continue;

                    full.Append(chunk);
                    RenderChunk(chunk);
                }

                Console.WriteLine(); // finish line

                // --- Save complete reply
                session.AddAssistantInteraction(
                    new Interaction("assistant", "text", new[] { new TextAttachment(full.ToString()) })
                );

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Error: {ex.Message}]\n");
                Console.ResetColor();
            }
        }
    }

    // Build a single Interaction whose single TextAttachment
    // contains the entire conversation history as:
    // system: ...
    // user: ...
    // assistant: ...
    // ending with "assistant:" to prompt continuation
    private static IInteraction BuildPromptFromHistory(IInteractionSession session)
    {
        var sb = new StringBuilder();
        foreach (var msg in session.History.Interactions)
        {
            var text = msg
              .Attachments
              .OfType<ITextAttachment>()
              .FirstOrDefault()?.Text;
            if (string.IsNullOrWhiteSpace(text)) continue;

            sb.AppendLine($"{msg.Role}: {text}");
        }
        sb.Append("assistant: ");
        return new Interaction("user", "text", new[] { new TextAttachment(sb.ToString()) });
    }

    // Renders a chunk, stripping out <think>...</think> tags completely,
    // printing only the inner text in DARK GRAY, and everything else in WHITE.
    private static void RenderChunk(string chunk)
    {
        var pattern = new Regex(@"<think>(.*?)</think>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        int idx = 0;

        foreach (Match m in pattern.Matches(chunk))
        {
            // 1) print text before this <think> in white
            if (m.Index > idx)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(chunk.Substring(idx, m.Index - idx));
            }

            // 2) print ONLY the inner think content, in dark gray
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(m.Groups[1].Value);
            Console.ResetColor();

            // move past this entire match
            idx = m.Index + m.Length;
        }

        // 3) any trailing text after the last </think> in white
        if (idx < chunk.Length)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(chunk.Substring(idx));
            Console.ResetColor();
        }
    }
}
