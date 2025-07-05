// IFileAttachment.cs

namespace SpireCore.AI.Interactions.Contracts.Attachments;

/// <summary>
/// File content attachment (image, file, audio, etc.).
/// </summary>
public interface IFileAttachment : IInteractionAttachment
{
    string FileName { get; }
    string MimeType { get; }
    string Base64ContentsOrUrl { get; }
    bool IsUrl { get; }
}