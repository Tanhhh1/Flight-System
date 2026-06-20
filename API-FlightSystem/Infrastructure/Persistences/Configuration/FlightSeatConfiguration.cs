using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Configuration
{
    public class FlightSeatConfiguration : IEntityTypeConfiguration<FlightSeat>
    {
        public void Configure(EntityTypeBuilder<FlightSeat> builder)
        {
            builder.HasKey(x => x.FlightSeatId);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.Flight)
                .WithMany(f => f.FlightSeats)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SeatTemplate)
                .WithMany()
                .HasForeignKey(x => x.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.FlightId, x.SeatId })
                .IsUnique()
                .HasDatabaseName("IX_FlightSeat_Unique");

            builder.HasIndex(x => new { x.FlightId, x.Status })
                .HasDatabaseName("IX_FlightSeat_Availability");
        }
    }
}
