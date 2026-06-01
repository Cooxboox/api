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

    builder.ToTable(nameof(CooxbooxContext.Kitchens), CooxbooxContext.Schema);
    builder.HasKey(x => x.KitchenId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.OwnerId);
    builder.HasIndex(x => x.Confidentiality);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Slug).IsUnique();

    builder.Property(x => x.OwnerId).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.Confidentiality).HasMaxLength(8).HasConversion(new EnumToStringConverter<Confidentiality>());
    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Slug).HasMaxLength(Slug.MaximumLength);
  }
}
