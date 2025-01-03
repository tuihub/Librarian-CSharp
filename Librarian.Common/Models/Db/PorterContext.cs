﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Librarian.Common.Models.Db
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class PorterContext
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = null!;
        [MaxLength(4095)]
        public string Description { get; set; } = null!;
        [MaxLength(65535)]
        public string ContextJson { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // relations
        // one-to-one relation(optional, to parent)
        public long PorterId { get; set; }
        public Porter Porter { get; set; } = null!;
    }
}
