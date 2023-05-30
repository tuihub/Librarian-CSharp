using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Sephirah.Models
{
    [Index(nameof(EntityInternalId))]
    public class GameSaveFileRotation
    {
        [Key]
        public long Id { get; set; }
        public long EntityInternalId { get; set; }
        public VaildScope VaildScope { get; set; }
        public long Count { get; set; }
    }

    public enum VaildScope
    {
        Account,
        App,
        AppPackage
    }
}
