using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class PlaneConfiguration : IEntityTypeConfiguration<Plane>
    {
        public void Configure(EntityTypeBuilder<Plane> builder)
        {
            builder.HasKey(x => x.PlaneId);

            builder.Property(x => x.PlaneModel)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.Airline)
                .WithMany(a => a.Planes)
                .HasForeignKey(x => x.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Flights)
                .WithOne(f => f.Plane)
                .HasForeignKey(f => f.PlaneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
