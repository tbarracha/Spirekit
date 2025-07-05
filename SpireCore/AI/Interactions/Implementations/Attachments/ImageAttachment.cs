using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations.Attachments;

public class ImageAttachment : FileAttachment, IImageAttachment
{
    public ImageAttachment(
        string fileName,
        string mimeType,
        string base64ContentsOrUrl,
        bool isUrl = false,
        string? label = null
    ) : base("image", fileName, mimeType, base64ContentsOrUrl, isUrl, label)
    {
    }
}
