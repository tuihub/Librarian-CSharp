using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Models
{
    [PrimaryKey(nameof(UserId), nameof(AppId), nameof(AppCategoryId))]
    public class UserAppAppCategory
    {
        // one-to-one relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-one relation(required, to parent)
        public long AppId { get; set; }
        public App App { get; set; } = null!;
        // one-to-one relation(required, to parent)
        public long AppCategoryId { get; set; }
        public AppCategory AppCategory { get; set; } = null!;
    }
}
