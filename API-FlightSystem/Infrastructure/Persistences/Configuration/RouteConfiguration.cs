using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(x => x.RouteId);

            builder.Property(x => x.FlightDuration)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.OriginAirport)
                .WithMany(a => a.OriginRoutes)
                .HasForeignKey(x => x.OriginAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationAirport)
                .WithMany(a => a.DestinationRoutes)
                .HasForeignKey(x => x.DestinationAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Flights)
                .WithOne(f => f.Route)
                .HasForeignKey(f => f.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.OriginAirportId, x.DestinationAirportId })
                .HasDatabaseName("IX_Route_Origin_Destination");
        }
    }
}
