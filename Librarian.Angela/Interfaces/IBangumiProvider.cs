using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Interfaces
{
    public interface IBangumiProvider
    {
        public Task PullAppInfoAsync(long internalID);
    }
}
