
using SpireCore.API.Services;

namespace SpireApi.Application.Services;

public class GoodbyeService : ITransientService
{
    public string GetGoodbye() => "Goodbye, World!";
    public string GetHello(string name) => $"Goodbye, {name}";
}
