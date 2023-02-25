using TuiHub.Protos.Librarian.V1;

namespace Librarian.Models
{
    public class App
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public AppSource Source { get; set; }
        public string SourceAppId { get; set; } = null!;
        public string? SourceUrl { get; set; }
        public string Name { get; set; } = null!;
        public AppType Type { get; set; }
        public string? ShortDescription { get; set; }
        public string? ImageUrl { get; set; }
        public AppDetails? AppDetails { get; set; }
    }
}
