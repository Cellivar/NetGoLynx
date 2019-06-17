using Microsoft.EntityFrameworkCore;
using NetGoLynx.Models;

namespace NetGoLynx.Data
{
    public class RedirectContext : DbContext
    {
        public RedirectContext(DbContextOptions<RedirectContext> options)
            : base(options) { }

        public DbSet<Redirect> Redirects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Redirect>()
                .HasIndex(r => r.Name);
        }
    }
}
