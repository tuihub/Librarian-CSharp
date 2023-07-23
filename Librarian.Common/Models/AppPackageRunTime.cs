using Google.Protobuf;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    public class AppPackageRunTime
    {
        // not InternalId
        [Key]
        public long Id { get; set; }
        // in relation
        //public long AppPackageId { get; set; }
        public DateTime StartDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        // one-to-one relation(required, to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
    }
}
