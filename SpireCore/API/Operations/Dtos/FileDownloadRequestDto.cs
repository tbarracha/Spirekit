
namespace SpireCore.API.Operations.Dtos;

public enum FileDownloadType
{
    /// <summary>Returns a DTO with Base64 content in the JSON body.</summary>
    Base64,
    /// <summary>Returns as a true file download (HTTP content-disposition: attachment).</summary>
    File
}
 
public class FileDownloadRequestDto
{
    public string FileName { get; set; } = default!;
    public FileDownloadType DownloadType { get; set; } = FileDownloadType.File;
}