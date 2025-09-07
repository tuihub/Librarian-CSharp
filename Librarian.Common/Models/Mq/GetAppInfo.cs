namespace Librarian.Common.Models.Mq;

public record GetAppInfo : PorterMessageBase
{
    public string Source { get; set; } = string.Empty;
    public string SourceAppId { get; set; } = string.Empty;
}