using Cooxboox.Core;
using Cooxboox.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cooxboox.Infrastructure.Configurations;

internal class IngredientTypeConfiguration : AggregateConfiguration<IngredientTypeEntity>, IEntityTypeConfiguration<IngredientTypeEntity>
{
  public override void Configure(EntityTypeBuilder<IngredientTypeEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(CooxbooxContext.IngredientTypes), Schemas.Content);
    builder.HasKey(x => x.IngredientTypeId);

    builder.HasIndex(x => new { x.KitchenId, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.Name);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.Kitchen).WithMany(x => x.IngredientTypes).OnDelete(DeleteBehavior.Restrict);
  }
}
