﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Models
{
    [Index(nameof(CreatedAt))]
    [Index(nameof(UpdatedAt))]
    public class App
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; } = null!;
        [MaxLength(1024)]
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // one-to-one relation(required, to child)
        public AppBinary? AppBinary { get; set; }
        // one-to-many relation(optional, to parent)
        public long? AppInfoId { get; set; }
        public AppInfo? AppInfo { get; set; }
        // func
        public App(long internalId, TuiHub.Protos.Librarian.Sephirah.V1.App app)
        {
            Id = internalId;
            Name = app.Name;
            Description = string.IsNullOrEmpty(app.Description) ? null : app.Description;
            IsPublic = app.Public;
            AppInfoId = app.AssignedAppInfoId?.Id;
        }
        public App() { }
        public TuiHub.Protos.Librarian.Sephirah.V1.App ToProtoApp()
        {
            return new TuiHub.Protos.Librarian.Sephirah.V1.App
            {
                Id = new InternalID { Id = Id },
                Name = Name,
                Description = Description,
                Public = IsPublic,
                AssignedAppInfoId = AppInfoId == null ? null : new InternalID { Id = (long)AppInfoId }
            };
        }
    }
}
