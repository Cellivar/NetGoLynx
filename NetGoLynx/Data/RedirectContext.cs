using Microsoft.EntityFrameworkCore;
using NetGoLynx.Models;

namespace NetGoLynx.Data
{
    /// <summary>
    /// Database interaction context for Redirects
    /// </summary>
    public class RedirectContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectContext"/> class.
        /// </summary>
        /// <param name="options"></param>
        public RedirectContext(DbContextOptions<RedirectContext> options)
            : base(options) { }

        /// <summary>
        /// Gets or sets the Redirects database object.
        /// </summary>
        public DbSet<Redirect> Redirects { get; set; }

        /// <summary>
        /// Event fired when the data model is being created.
        /// </summary>
        /// <param name="modelBuilder">The model builder object being created.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Redirect>()
                .HasIndex(r => r.Name);
        }
    }
}
