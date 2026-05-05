using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.HasKey(x => x.FlightId);

            builder.Property(x => x.DepartureTime)
                .IsRequired();

            builder.Property(x => x.ArrivalTime)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.Plane)
                .WithMany(p => p.Flights)
                .HasForeignKey(x => x.PlaneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Route)
                .WithMany(r => r.Flights)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Policy)
                .WithMany(p => p.Flights)
                .HasForeignKey(x => x.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.FlightServices)
                .WithOne(s => s.Flight)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.FlightSeats)
                .WithOne(s => s.Flight)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Bookings)
                .WithOne(b => b.Flight)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.DepartureTime, x.RouteId })
                .HasDatabaseName("IX_Flight_Departure_Route");
        }
    }
}
