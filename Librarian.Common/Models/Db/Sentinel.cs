using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1.Sentinel;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Sentinel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(511)] public string Url { get; set; } = null!;
        [MaxLength(4095)] public ICollection<string> AltUrls { get; set; } = [];
        [MaxLength(511)] public string GetTokenUrlPath { get; set; } = null!;
        [MaxLength(511)] public string DownloadFileUrlPath { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // relations
        // one-to-many relation(required, to child)
        public ICollection<SentinelLibrary> SentinelLibraries { get; } = [];

        // functions
        public Sentinel() { }
        public Sentinel(long id, ReportSentinelInformationRequest request)
        {
            Id = id;
            Url = request.Url;
            AltUrls = request.UrlAlternatives;
            GetTokenUrlPath = request.GetTokenPath;
            DownloadFileUrlPath = request.DownloadFileBasePath;
            foreach (var lib in request.Libraries)
            {
                SentinelLibraries.Add(new SentinelLibrary(id, lib));
            }
        }
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
                {
                    existingLib.Update(lib);
                }
                else
                {
                    SentinelLibraries.Add(new SentinelLibrary(Id, lib));
                }
            }
        }
    }
}
