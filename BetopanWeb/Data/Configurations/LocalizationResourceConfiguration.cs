using BetopanWeb.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetopanWeb.Data.Configurations
{
    public class LocalizationResourceConfiguration : IEntityTypeConfiguration<LocalizationResource>
    {
     
        public void Configure(EntityTypeBuilder<LocalizationResource> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ResourceKey)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.LanguageCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Value)
                .IsRequired();

            builder.HasIndex(x => new { x.ResourceKey, x.LanguageCode })
                .IsUnique();

            builder.HasIndex(x => x.LanguageCode);
        }
    }
}
