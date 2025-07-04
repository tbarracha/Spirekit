namespace SpireCore.AI.Interactions.Attachments;

/// <summary>
/// File content attachment (image, file, audio, etc.).
/// </summary>
public class FileAttachment : IFileAttachment
{
    public string Type => "file";
    public string? Label { get; }
    public string FileName { get; }
    public string MimeType { get; }
    public string Base64ContentsOrUrl { get; }
    public bool IsUrl { get; }

    public FileAttachment(string fileName, string mimeType, string base64ContentsOrUrl, bool isUrl, string? label = null)
    {
        FileName = fileName;
        MimeType = mimeType;
        Base64ContentsOrUrl = base64ContentsOrUrl;
        IsUrl = isUrl;
        Label = label;
    }
}
