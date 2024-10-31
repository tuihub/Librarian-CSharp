using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Models
{
    public record AppIdMQ
    {
        public string AppId { get; set; } = null!;
        public bool UpdateInternalAppInfoName { get; set; }
    }
}
