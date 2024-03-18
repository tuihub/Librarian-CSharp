using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter
{
    public static class StaticContext
    {
        public static long? SephirahId { get; set; }
        public static List<string> PorterTags { get; } = new List<string>();
        public static Guid ConsulRegId { get; } = Guid.NewGuid();
    }
}
