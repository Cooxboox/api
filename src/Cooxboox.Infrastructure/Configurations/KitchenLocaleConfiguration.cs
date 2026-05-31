using Cooxboox.Core.Localization;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cooxboox.Infrastructure.Configurations;

internal class KitchenLocaleConfiguration : IEntityTypeConfiguration<KitchenLocaleEntity>
{
  public void Configure(EntityTypeBuilder<KitchenLocaleEntity> builder)
  {
    builder.ToTable(nameof(CooxbooxContext.KitchenLocales), CooxbooxContext.Schema);
    builder.HasKey(x => x.KitchenLocaleId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => new { x.KitchenId, x.Language }).IsUnique();
    builder.HasIndex(x => x.Language);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);

    builder.Property(x => x.Language).HasMaxLength(Language.MaximumLength);
    // TODO(fpion): MetaDescription should have a limit of 160 characters.
    builder.Property(x => x.CreatedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.UpdatedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Kitchen).WithMany(x => x.Locales).OnDelete(DeleteBehavior.Cascade);
  }
}
