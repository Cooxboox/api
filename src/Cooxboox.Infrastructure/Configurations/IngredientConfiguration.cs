using Cooxboox.Core;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooxboox.Infrastructure.Configurations;

internal class IngredientConfiguration : AggregateConfiguration<IngredientEntity>, IEntityTypeConfiguration<IngredientEntity>
{
  public override void Configure(EntityTypeBuilder<IngredientEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(CooxbooxContext.Ingredients), Schemas.Content);
    builder.HasKey(x => x.IngredientId);

    builder.HasIndex(x => new { x.KitchenId, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.PublishedVersion);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(16).HasConversion(new EnumToStringConverter<ContentStatus>());
    builder.Property(x => x.PublishedBy).HasMaxLength(ActorId.MaximumLength);

    builder.HasOne(x => x.Kitchen).WithMany(x => x.Ingredients).OnDelete(DeleteBehavior.Restrict);
  }
}
