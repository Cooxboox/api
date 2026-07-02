using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Validation;
using Cooxboox.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooxboox.Infrastructure.Configurations;

internal class KitchenLocaleConfiguration : IEntityTypeConfiguration<KitchenLocale>
{
  public void Configure(EntityTypeBuilder<KitchenLocale> builder)
  {
    builder.ToTable(nameof(CooxbooxContext.KitchenLocales), Schemas.Content);
    builder.HasKey(x => new { x.KitchenId, x.Language });

    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.Language).HasMaxLength(Language.MaximumLength);
    builder.Property(x => x.MetaDescription).HasMaxLength(Constants.MetaDescriptionMaximumLength);
    builder.Property(x => x.Status).HasMaxLength(16).HasConversion(new EnumToStringConverter<ContentStatus>());

    builder.HasOne(x => x.Kitchen).WithMany(x => x.Locales).OnDelete(DeleteBehavior.Cascade);
  }
}
