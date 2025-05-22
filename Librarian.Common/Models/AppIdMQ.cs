namespace Librarian.Common.Models
{
    public record AppIdMQ
    {
        public string AppId { get; set; } = null!;
        public bool UpdateInternalAppInfoName { get; set; }
    }
}
