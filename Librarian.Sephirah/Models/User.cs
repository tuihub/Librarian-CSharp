using System;
using System.Collections.Generic;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserType Type { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // many-to-many relation(wihtout entity, to other parent)
        public ICollection<App> Apps { get; } = new List<App>();
    }
}
