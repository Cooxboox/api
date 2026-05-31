using Cooxboox.Core.Localization;
using Cooxboox.Core.Seo;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cooxboox.Infrastructure.Configurations;

internal class PublishedKitchenLocaleConfiguration : IEntityTypeConfiguration<PublishedKitchenLocaleEntity>
{
  public void Configure(EntityTypeBuilder<PublishedKitchenLocaleEntity> builder)
  {
    builder.ToTable(nameof(CooxbooxContext.PublishedKitchenLocales), CooxbooxContext.Schema);
    builder.HasKey(x => x.KitchenLocaleId);

    builder.HasIndex(x => new { x.KitchenId, x.Language }).IsUnique();
    builder.HasIndex(x => x.Language);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.Language).HasMaxLength(Language.MaximumLength);
    builder.Property(x => x.MetaDescription).HasMaxLength(MetaDescription.MaximumLength);
    builder.Property(x => x.PublishedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.KitchenLocale).WithOne(x => x.PublishedLocale)
      .HasPrincipalKey<KitchenLocaleEntity>(x => x.KitchenLocaleId)
      .HasForeignKey<PublishedKitchenLocaleEntity>(x => x.KitchenLocaleId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Kitchen).WithMany(x => x.PublishedLocales).OnDelete(DeleteBehavior.Cascade);
  }
}
