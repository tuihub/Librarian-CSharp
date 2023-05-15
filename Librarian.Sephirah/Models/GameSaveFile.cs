﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Librarian.Sephirah.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class GameSaveFile
    {
        // same InternalId as FileMeta
        [Key]
        public long Id { get; set; }
        public GameSaveFileStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to parent)
        public FileMetadata FileMetadata { get; set; } = null!;
        // one-to-many relation(to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
    public enum GameSaveFileStatus
    {
        PENDING,
        IN_PROGRESS,
        STORED,
        SHA256_MISMATCH,
        FAILED,
    }
}