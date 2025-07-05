using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations.Attachments;

public class VideoAttachment : FileAttachment, IVideoAttachment
{
    public VideoAttachment(
        string fileName,
        string mimeType,
        string base64ContentsOrUrl,
        bool isUrl = false,
        string? label = null
    ) : base("video", fileName, mimeType, base64ContentsOrUrl, isUrl, label)
    {
    }
}
