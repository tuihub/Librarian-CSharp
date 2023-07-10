﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
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
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(required, to child)
        public ICollection<UserAppAppCategory> UserAppAppCategories { get; } = new List<UserAppAppCategory>();
        public AppCategory() { }
        public TuiHub.Protos.Librarian.V1.AppCategory ToProtoAppCategory()
        {
            return new TuiHub.Protos.Librarian.V1.AppCategory
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = Id },
                Name = Name
            };
        }
    }
}
