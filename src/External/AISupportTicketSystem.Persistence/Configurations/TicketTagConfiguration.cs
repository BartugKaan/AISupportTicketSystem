using AISupportTicketSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AISupportTicketSystem.Persistence.Configurations;

public class TicketTagConfiguration : IEntityTypeConfiguration<TicketTag>
{
    public void Configure(EntityTypeBuilder<TicketTag> builder)
    {
        builder.ToTable("TicketTags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Color)
            .HasMaxLength(20);

        builder.HasOne(t => t.Ticket)
            .WithMany(ticket => ticket.Tags)
            .HasForeignKey(t => t.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.TicketId, t.Name })
            .IsUnique();

        builder.HasQueryFilter(t => !t.IsDeleted);

    }
}