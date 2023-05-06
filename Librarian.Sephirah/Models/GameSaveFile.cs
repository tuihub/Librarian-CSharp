﻿namespace Librarian.Sephirah.Models
{
    public class GameSaveFile
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public GameSaveFileStatus Status { get; set; }
        // relations
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long AppId { get; set; }
        public App App { get; set; } = null!;
    }
    public enum GameSaveFileStatus
    {
        PENDING,
        IN_PROGRESS,
        STORED,
        FAILED,
    }
}
