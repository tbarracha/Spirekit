namespace SpireApi.Shared.Operations.Dtos;

public class FileDownloadResponseDto
{
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public string Base64Data { get; set; } = default!;
    // Optionally, add a length property, etc.
}
