using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // many-to-many relation(optional)
        public ICollection<App> Apps { get; } = [];

        // functions
        public AppCategory() { }
        // without app relations
        public AppCategory(long id, long userId, TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppCategory appCategory)
        {
            Id = id;
            Name = appCategory.Name;
            UserId = userId;
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppCategory ToPB()
        {
            return StaticContext.Mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.Sephirah.AppCategory>(this);
        }
    }
}
