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

            builder.Property(x => x.OriginAirportId)
                .IsRequired();

            builder.Property(x => x.DestinationAirportId)
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

            builder.HasOne(x => x.OriginAirport)
                .WithMany()
                .HasForeignKey(x => x.OriginAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationAirport)
                .WithMany()
                .HasForeignKey(x => x.DestinationAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.FlightId, x.SegmentOrder })
                .IsUnique()
                .HasDatabaseName("UX_Flight_SegmentOrder");
        }
    }
}
