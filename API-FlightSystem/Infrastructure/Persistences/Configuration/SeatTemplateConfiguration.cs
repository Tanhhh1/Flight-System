using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class SeatTemplateConfiguration : IEntityTypeConfiguration<SeatTemplate>
    {
        public void Configure(EntityTypeBuilder<SeatTemplate> builder)
        {
            builder.HasKey(x => x.SeatId);

            builder.Property(x => x.SeatNumber)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.RowIndex)
                .IsRequired();

            builder.Property(x => x.ColIndex)
                .IsRequired();

            builder.HasOne(x => x.SeatClass)
                .WithMany(c => c.SeatTemplates)
                .HasForeignKey(x => x.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.FlightSeats)
                .WithOne(fs => fs.SeatTemplate)
                .HasForeignKey(fs => fs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SeatNumber);

            builder.HasIndex(x => new { x.RowIndex, x.ColIndex }).IsUnique();
        }
    }
}
