using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Configuration
{
    public class FlightSegmentConfiguration : IEntityTypeConfiguration<FlightSegment>
    {
        public void Configure(EntityTypeBuilder<FlightSegment> builder)
        {
            builder.HasKey(x => x.SegmentId);

            builder.Property(x => x.RouteId)
                .IsRequired();

            builder.Property(x => x.DepartureTime)
                .IsRequired();

            builder.Property(x => x.ArrivalTime)
                .IsRequired();

            builder.Property(x => x.SegmentOrder)
                .IsRequired();

            builder.HasOne(x => x.Flight)
                .WithMany(f => f.FlightSegments)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Route)
                .WithMany(r => r.FlightSegments)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.FlightId, x.SegmentOrder })
                .IsUnique()
                .HasDatabaseName("UX_Flight_SegmentOrder");
        }
    }
}
