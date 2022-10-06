using System;
using System.Collections.Generic;

namespace Librarian.Models
{
    public class User
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public StatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public enum StatusEnum { Activated, Blocked }
    }
}
