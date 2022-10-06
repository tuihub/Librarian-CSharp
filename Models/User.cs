using System;
using System.Collections.Generic;

namespace Librarian.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
