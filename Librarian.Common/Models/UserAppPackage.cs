﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [PrimaryKey(nameof(UserId), nameof(AppPackageId))]
    public class UserAppPackage
    {
        // one-to-one relation(required, to parent)
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        // one-to-one relation(required, to parent)
        public long AppPackageId { get; set; }
        public AppPackage AppPackage { get; set; } = null!;
        public TimeSpan TotalRunTime { get; set; } = TimeSpan.Zero;
    }
}
