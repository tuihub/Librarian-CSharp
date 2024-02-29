using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class AppCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // many-to-many relation(optional)
        public ICollection<AppInfo> AppInfos { get; } = new List<AppInfo>();
        // many-to-many relation(optional)
        public ICollection<App> Apps { get; } = new List<App>();
        public AppCategory() { }
        public AppCategory(long id, long userId, TuiHub.Protos.Librarian.Sephirah.V1.AppCategory appCategory)
        {
            Id = id;
            Name = appCategory.Name;
            UserId = userId;
        }
        public TuiHub.Protos.Librarian.Sephirah.V1.AppCategory ToProtoAppCategory()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.AppCategory
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Name = Name
            };
        }
    }
}
