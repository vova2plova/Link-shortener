using LinkShortener.API.Infrastructure.EntityConfigurations;

namespace LinkShortener.API.Infrastructure
{
    public class LinkContext(DbContextOptions<LinkContext> options) : DbContext(options)
    {
        public DbSet<Link> Links { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LinkTypeConfiguration());
            modelBuilder.Entity<Link>()
                .HasQueryFilter(x => x.IsDeleted == false);
        }
    }
}
