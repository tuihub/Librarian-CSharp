using System;
using System.Collections.Generic;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
{
    public class User
    {
        public long Id { get; set; }
        public long InternalId { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserType Type { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // many-to-many relations
        public List<App> Apps { get; } = new();
    }
}
