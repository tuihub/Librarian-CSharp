using Librarian.Sephirah.Models;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Sephirah.Utils
{
    public class TestDbContext : DbContext
    {
        public TestDbContext() { }
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<App> Apps { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlite("Data Source=./test.db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                //e.ToTable("user");

                //e.Property(e => e.Id)
                //    .IsRequired()
                //    .HasColumnName("id");

                //e.Property(e => e.InternalId)
                //    .IsRequired()
                //    .HasColumnName("internal_id");

                //e.Property(e => e.UserName)
                //    .IsRequired()
                //    .HasColumnName("username");
            });
        }
    }
}
