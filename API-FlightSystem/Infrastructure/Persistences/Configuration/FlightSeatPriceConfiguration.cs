using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class FlightSeatPriceConfiguration : IEntityTypeConfiguration<FlightSeatPrice>
    {
        public void Configure(EntityTypeBuilder<FlightSeatPrice> builder)
        {
            builder.HasKey(x => new { x.FlightId, x.ClassId });

            builder.Property(x => x.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.AvailableSeats)
                .IsRequired();

            builder.HasOne(x => x.Flight)
                .WithMany(f => f.FlightSeatPrices)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SeatClass)
                .WithMany()
                .HasForeignKey(x => x.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
