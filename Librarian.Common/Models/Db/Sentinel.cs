using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sentinel.V1;

namespace Librarian.Common.Models.Db;

[Index(nameof(UserId))]
[Index(nameof(CreatedAt))]
[Index(nameof(UpdatedAt))]
public class Sentinel
{
    // functions
    public Sentinel()
    {
    }

    public Sentinel(long id, ReportSentinelInformationRequest request)
    {
        Id = id;
        Url = request.Url;
        AltUrls = request.UrlAlternatives;
        GetTokenUrlPath = request.GetTokenPath;
        DownloadFileUrlPath = request.DownloadFileBasePath;
        foreach (var lib in request.Libraries) SentinelLibraries.Add(new SentinelLibrary(id, lib));
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long Id { get; set; }

    [MaxLength(511)] public string Url { get; set; } = null!;
    [MaxLength(4095)] public ICollection<string> AltUrls { get; set; } = [];
    [MaxLength(511)] public string GetTokenUrlPath { get; set; } = null!;
    [MaxLength(511)] public string DownloadFileUrlPath { get; set; } = null!;
    [MaxLength(1023)] public string RefreshToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // relations
    // one-to-many relation(required, to parent)
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    // one-to-many relation(required, to child)
    public ICollection<SentinelLibrary> SentinelLibraries { get; } = [];

    public void Update(ReportSentinelInformationRequest request)
    {
        Url = request.Url;
        AltUrls = request.UrlAlternatives;
        GetTokenUrlPath = request.GetTokenPath;
        DownloadFileUrlPath = request.DownloadFileBasePath;
        UpdatedAt = DateTime.UtcNow;
        var existingLibraryIds = SentinelLibraries.Select(sl => sl.LibraryId).ToHashSet();
        foreach (var lib in request.Libraries)
        {
            var existingLib = SentinelLibraries.FirstOrDefault(sl => sl.LibraryId == lib.Id);
            if (existingLib != null)
                existingLib.Update(lib);
            else
                SentinelLibraries.Add(new SentinelLibrary(Id, lib));
        }
    }
}