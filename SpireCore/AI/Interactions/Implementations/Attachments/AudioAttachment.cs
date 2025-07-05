using SpireCore.AI.Interactions.Contracts.Attachments;

namespace SpireCore.AI.Interactions.Implementations.Attachments;

public class AudioAttachment : FileAttachment, IAudioAttachment
{
    public AudioAttachment(
        string fileName,
        string mimeType,
        string base64ContentsOrUrl,
        bool isUrl = false,
        string? label = null
    ) : base("audio", fileName, mimeType, base64ContentsOrUrl, isUrl, label)
    {
    }
}
