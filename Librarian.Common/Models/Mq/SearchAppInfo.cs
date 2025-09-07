namespace Librarian.Common.Models.Mq;

public record SearchAppInfo : PorterMessageBase
{
    public string Source { get; set; } = string.Empty;
    public string NameLike { get; set; } = string.Empty;
}