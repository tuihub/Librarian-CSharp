using Google.Protobuf;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    public class AppInstRunTime
    {
        // not InternalId, database generated
        [Key]
        public long Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
