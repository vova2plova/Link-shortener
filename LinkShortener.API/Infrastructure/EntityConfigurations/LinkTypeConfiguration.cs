using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkShortener.API.Infrastructure.EntityConfigurations
{
    public class LinkTypeConfiguration
        : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.ToTable("Link");

            builder.Property(l => l.Suffix)
                .HasMaxLength(6);
        }
    }
}
