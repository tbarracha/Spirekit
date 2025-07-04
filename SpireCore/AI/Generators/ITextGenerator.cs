using SpireCore.AI.Clients;

namespace SpireCore.AI.Generators;

public interface ITextGenerator
{
    Task<string> GenerateTextAsync(string prompt, AiClientConfiguration? config);
}
