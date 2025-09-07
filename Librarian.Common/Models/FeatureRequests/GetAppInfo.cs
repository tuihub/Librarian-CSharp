using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.FeatureRequests;

public class GetAppInfo
{
    [Required] [MaxLength(511)] public string AppId { get; set; } = null!;
}