using System.ComponentModel;

namespace SpireApi.Contracts.Hello;

public class HelloRequest
{
    [DefaultValue("Willy")]
    public string Name { get; set; } = string.Empty;

    [DefaultValue("Wonka")]
    public string LastName { get; set; } = string.Empty;
}
