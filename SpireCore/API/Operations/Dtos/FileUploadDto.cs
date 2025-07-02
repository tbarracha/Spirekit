
namespace SpireCore.API.Operations.Dtos;

public class FileUploadDto
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public string Base64Data { get; set; } = default!;
    // Optionally, you could use byte[] instead of Base64Data if you use [FromBody] and set up serialization.
}
