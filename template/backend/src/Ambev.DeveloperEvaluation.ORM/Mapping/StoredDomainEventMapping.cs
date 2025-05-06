using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class StoredDomainEventMapping : IEntityTypeConfiguration<StoredDomainEvent>
{
    public void Configure(EntityTypeBuilder<StoredDomainEvent> builder)
    {
        builder.ToTable("StoredDomainEvents");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.EventType).IsRequired();
        builder.Property(e => e.EventData).IsRequired();
        builder.Property(e => e.Timestamp).IsRequired();
        builder.Property(e => e.SaleId).IsRequired(false);
        builder.Property(e => e.BranchId).IsRequired(false);
        builder.Property(e => e.CustomerId).IsRequired(false);
    }
}