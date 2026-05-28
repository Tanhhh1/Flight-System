using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Configuration
{
    public class AirlineConfiguration : IEntityTypeConfiguration<Airline>
    {
        public void Configure(EntityTypeBuilder<Airline> builder) 
        { 
            builder.HasKey(a => a.AirlineId);

            builder.Property(a => a.AirlineName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.AirlineCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(a => a.AirlineCode)
                .IsUnique();

            builder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Status)
                .IsRequired();

            builder.HasMany(x => x.Planes)
                .WithOne(p => p.Airline)
                .HasForeignKey(p => p.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
