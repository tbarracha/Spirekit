using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations.Attachments;

public class FileAttachment : InteractionAttachment, IFileAttachment
{
    public string FileName { get; }
    public string MimeType { get; }
    public string Base64ContentsOrUrl { get; }
    public bool IsUrl { get; }

    public FileAttachment(
        string type,
        string fileName,
        string mimeType,
        string base64ContentsOrUrl,
        bool isUrl,
        string? label = null
    ) : base(type, label)
    {
        FileName = fileName;
        MimeType = mimeType;
        Base64ContentsOrUrl = base64ContentsOrUrl;
        IsUrl = isUrl;
    }
}
