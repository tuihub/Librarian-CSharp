﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Common.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class Sentinel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Token { get; set; } = null!;
        //public ICollection<string> CdnUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // relations
        // one-to-many relation(required, to child)
        public ICollection<AppBinary> AppBinaries { get; } = new List<AppBinary>();
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
