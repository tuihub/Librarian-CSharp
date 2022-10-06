using System;
using System.Collections.Generic;

namespace Librarian.Models
{
    public partial class Game
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? SavedataLastModified { get; set; }
    }
}
