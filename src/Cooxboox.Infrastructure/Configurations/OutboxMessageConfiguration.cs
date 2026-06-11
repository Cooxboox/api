using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cooxboox.Infrastructure.Configurations;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessageEntity>
{
  public void Configure(EntityTypeBuilder<OutboxMessageEntity> builder)
  {
    builder.ToTable(nameof(CooxbooxContext.OutboxMessages));
    builder.HasKey(x => x.OutboxMessageId);

    builder.HasIndex(x => x.StreamId);
    builder.HasIndex(x => x.EventId);
    builder.HasIndex(x => x.Version);
    builder.HasIndex(x => x.UpdatedOn);
    builder.HasIndex(x => x.Status);

    builder.Property(x => x.StreamId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.EventId).HasMaxLength(EventId.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(16).HasConversion(new EnumToStringConverter<OutboxMessageStatus>());
  }
}
