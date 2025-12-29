using AISupportTicketSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISupportTicketSystem.Persistence.Configurations;

public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(EntityTypeBuilder<TicketHistory> builder)
    {
        builder.ToTable("TicketHistories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.OldValue)
            .HasMaxLength(1000);

        builder.Property(h => h.NewValue)
            .HasMaxLength(1000);

        builder.Property(h => h.Description)
            .HasMaxLength(500);

        builder.HasOne(h => h.Ticket)
            .WithMany(t => t.History)
            .HasForeignKey(h => h.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.PerformedBy)
            .WithMany()
            .HasForeignKey(h => h.PerformedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(h => h.TicketId);
        builder.HasIndex(h => h.CreatedAt);

        builder.HasQueryFilter(h => !h.IsDeleted);
    }
}