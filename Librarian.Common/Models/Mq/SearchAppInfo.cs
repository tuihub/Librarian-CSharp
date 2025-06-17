namespace Librarian.Common.Models.Mq
{
    public record SearchAppInfo : PorterMessageBase
    {
        public string NameLike { get; set; } = string.Empty;
    }
}