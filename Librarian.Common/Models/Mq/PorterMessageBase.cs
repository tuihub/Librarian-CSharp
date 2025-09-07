namespace Librarian.Common.Models.Mq;

public abstract record PorterMessageBase
{
    public string? Region { get; set; }
    public Guid RequestId { get; set; } = Guid.NewGuid();
}