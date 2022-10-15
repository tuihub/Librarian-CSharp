using System;
using System.Collections.Generic;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Models
{
    public class User
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserType Type { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public enum StatusEnum { Activated, Blocked }
    }
}
