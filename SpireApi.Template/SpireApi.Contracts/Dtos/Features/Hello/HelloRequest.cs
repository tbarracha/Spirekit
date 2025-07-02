using System.ComponentModel;

namespace SpireApi.Contracts.Dtos.Features.Hello;

public class HelloRequest
{
    [DefaultValue("Bruce")]
    public string Name { get; set; } = string.Empty;

    [DefaultValue("Wayne")]
    public string LastName { get; set; } = string.Empty;
}
