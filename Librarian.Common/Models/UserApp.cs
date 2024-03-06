using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [PrimaryKey(nameof(UserId), nameof(AppId))]
    public class UserApp
    {
        // one-to-many relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-many relation(required, to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        // aggregations
        public TimeSpan TotalRunTime { get; set; } = TimeSpan.Zero;
    }
}
