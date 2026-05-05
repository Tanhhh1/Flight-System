using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class FlightServiceConfiguration : IEntityTypeConfiguration<FlightService>
    {
        public void Configure(EntityTypeBuilder<FlightService> builder)
        {
            builder.HasKey(x => new { x.FlightId, x.ServiceId });

            builder.HasOne(x => x.Flight)
                .WithMany(f => f.FlightServices)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Service)
                .WithMany(s => s.FlightServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    } 
}
