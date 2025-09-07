namespace Librarian.Common.Models.Mq;

public record ParseRawAppInfo : PorterMessageBase
{
    public string Source { get; set; } = string.Empty;
    public string SourceAppId { get; set; } = string.Empty;
    public string RawDataJson { get; set; } = string.Empty;
}