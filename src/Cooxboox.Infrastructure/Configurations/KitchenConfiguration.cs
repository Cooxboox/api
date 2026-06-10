using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Seo;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooxboox.Infrastructure.Configurations;

internal class KitchenConfiguration : AggregateConfiguration<KitchenEntity>, IEntityTypeConfiguration<KitchenEntity>
{
  public override void Configure(EntityTypeBuilder<KitchenEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(CooxbooxContext.Kitchens), Schemas.Content);
    builder.HasKey(x => x.KitchenId);

    builder.HasIndex(x => x.EntityId).IsUnique();
    builder.HasIndex(x => x.OwnerId);
    builder.HasIndex(x => x.Confidentiality);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Slug).IsUnique();
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.PublishedVersion);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.OwnerId).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.Confidentiality).HasMaxLength(8).HasConversion(new EnumToStringConverter<Confidentiality>());
    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Slug).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(16).HasConversion(new EnumToStringConverter<ContentStatus>());
    builder.Property(x => x.PublishedBy).HasMaxLength(ActorId.MaximumLength);
  }
}
