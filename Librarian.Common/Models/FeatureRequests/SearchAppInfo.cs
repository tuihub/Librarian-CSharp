using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.FeatureRequests;

public class SearchAppInfo
{
    [Required] [MaxLength(511)] public string NameLike { get; set; } = null!;
}