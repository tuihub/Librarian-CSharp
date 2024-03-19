using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Models
{
    public class AppIdMQ
    {
        public string AppId { get; set; } = null!;
        public bool UpdateInternalAppInfoName { get; set; }
        public override string ToString()
        {
            return $"{{AppId = {AppId}, UpdateInternalAppInfoName = {UpdateInternalAppInfoName}}}";
        }
    }
}
