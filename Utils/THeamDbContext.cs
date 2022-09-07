using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Librarian_CSharp.Models;

namespace Librarian_CSharp.Utils
{
    public partial class THeamDbContext : DbContext
    {
        public THeamDbContext()
        {
        }

        public THeamDbContext(DbContextOptions<THeamDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Games { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(GlobalContext.DB_CONN_STR, ServerVersion.Parse("10.3.34-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("game");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(128)
                    .HasColumnName("name");

                entity.Property(e => e.SavedataLastModified)
                    .HasColumnType("datetime")
                    .HasColumnName("savedata_last_modified");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(64)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(128)
                    .HasColumnName("password_hash");

                entity.Property(e => e.Token)
                    .HasMaxLength(128)
                    .HasColumnName("token");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
