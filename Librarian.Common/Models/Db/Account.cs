using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(Platform), nameof(PlatformAccountId), IsUnique = true)]
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)] public string Platform { get; set; } = null!;
        [MaxLength(255)] public string PlatformAccountId { get; set; } = null!;
        [MaxLength(255)] public string Name { get; set; } = null!;
        [MaxLength(255)] public string ProfileUrl { get; set; } = null!;
        [MaxLength(255)] public string AvatarUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(to child, only used in non-internal appInfo)
        public ICollection<AppInfo> AppInfos { get; } = [];
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        // functions
        public TuiHub.Protos.Librarian.Sephirah.V1.Account ToPb()
        {
            return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.Account>(this);
        }
    }
}
