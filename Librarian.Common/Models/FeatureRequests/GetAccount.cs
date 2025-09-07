using System.ComponentModel.DataAnnotations;

namespace Librarian.Common.Models.FeatureRequests;

public class GetAccount
{
    [Required] [MaxLength(511)] public string AccountId { get; set; } = null!;
}