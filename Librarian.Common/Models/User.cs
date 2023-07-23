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
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        [MaxLength(128)]
        public string Password { get; set; } = null!;
        public UserType Type { get; set; }
        public UserStatus Status { get; set; }
        public long GameSaveFileUsedCapacityBytes { get; set; } = 0;
        public long? GameSaveFileCapacityBytes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-many relation(required, to child)
        public ICollection<AppCategory> AppCategories { get; } = new List<AppCategory>();
        // one-to-many relation(required, to child)
        public ICollection<UserAppAppCategory> UserAppAppCategories { get; } = new List<UserAppAppCategory>();
        // many-to-many relation(wihtout entity, to other parent)
        public ICollection<App> Apps { get; } = new List<App>();
    }
}
