using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Configuration
{
    public class AirportConfiguration : IEntityTypeConfiguration<Airport>
    {
        public void Configure(EntityTypeBuilder<Airport> builder)
        {
            builder.HasKey(x => x.AirportId);

            builder.Property(x => x.AirportCode)
                .HasMaxLength(10) 
                .IsFixedLength()   
                .IsRequired();

            builder.Property(x => x.AirportName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(x => x.OriginRoutes)
                .WithOne(r => r.OriginAirport)
                .HasForeignKey(r => r.OriginAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DestinationRoutes)
                .WithOne(r => r.DestinationAirport)
                .HasForeignKey(r => r.DestinationAirportId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
