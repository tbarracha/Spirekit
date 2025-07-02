
namespace SpireCore.API.Operations.Dtos;

public class StreamableDto
{
    public string FileName { get; set; } = default!;
    public string? ContentType { get; set; }
    public Stream Content { get; set; } = default!; // This is not directly serializable; see usage below!
}
