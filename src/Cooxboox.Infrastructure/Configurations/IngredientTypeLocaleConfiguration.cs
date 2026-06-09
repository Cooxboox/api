using Cooxboox.Core;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Seo;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooxboox.Infrastructure.Configurations;

internal class IngredientTypeLocaleConfiguration : IEntityTypeConfiguration<IngredientTypeLocaleEntity>
{
  public void Configure(EntityTypeBuilder<IngredientTypeLocaleEntity> builder)
  {
    builder.ToTable(nameof(CooxbooxContext.IngredientTypeLocales), Schemas.Content);
    builder.HasKey(x => new { x.IngredientTypeId, x.Language });

    builder.HasIndex(x => x.Language);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => new { x.KitchenId, x.Language, x.Slug }).IsUnique();
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.PublishedVersion);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.Language).HasMaxLength(Language.MaximumLength);
    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Slug).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.MetaDescription).HasMaxLength(MetaDescription.MaximumLength);
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(16).HasConversion(new EnumToStringConverter<ContentStatus>());
    builder.Property(x => x.PublishedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.IngredientType).WithMany(x => x.Locales).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Kitchen).WithMany(x => x.IngredientTypeLocales).OnDelete(DeleteBehavior.Restrict);
  }
}
