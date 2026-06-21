using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooxboox.Infrastructure.Configurations;

internal class KitchenConfiguration : IEntityTypeConfiguration<Kitchen>
{
  public void Configure(EntityTypeBuilder<Kitchen> builder)
  {
    builder.ToTable(nameof(CooxbooxContext.Kitchens), Schemas.Content);
    builder.HasKey(x => x.KitchenId);

    builder.HasIndex(x => x.EntityId).IsUnique();
    builder.HasIndex(x => x.OwnerId);
    builder.HasIndex(x => x.Confidentiality);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Slug).IsUnique();
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.CreatedBy);
    builder.HasIndex(x => x.CreatedOn);
    builder.HasIndex(x => x.UpdatedBy);
    builder.HasIndex(x => x.UpdatedOn);
    builder.HasIndex(x => x.Status);
    builder.HasIndex(x => x.PublishedBy);
    builder.HasIndex(x => x.PublishedOn);

    builder.Property(x => x.Confidentiality).HasMaxLength(8).HasConversion(new EnumToStringConverter<Confidentiality>());
    builder.Property(x => x.Name).HasMaxLength(100); // TODO(fpion): constant
    builder.Property(x => x.Slug).HasMaxLength(100); // TODO(fpion): constant
    builder.Property(x => x.Status).HasMaxLength(16).HasConversion(new EnumToStringConverter<ContentStatus>());
  }
}
