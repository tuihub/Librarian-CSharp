namespace Librarian.Porter.Models;

public class InstanceContext
{
    public long? SephirahId { get; set; }
    public Guid ConsulRegId { get; } = Guid.NewGuid();
    public List<string> SupportedAccountPlatforms { get; } = [];
    public List<string> SupportedAppInfoSources { get; } = [];
}