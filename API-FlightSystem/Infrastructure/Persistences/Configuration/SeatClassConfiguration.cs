using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class SeatClassConfiguration : IEntityTypeConfiguration<SeatClass>
    {
        public void Configure(EntityTypeBuilder<SeatClass> builder)
        {
            builder.HasKey(x => x.ClassId);

            builder.Property(x => x.ClassName)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasMany(x => x.SeatTemplates)
                .WithOne(t => t.SeatClass)
                .HasForeignKey(t => t.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.FlightSeatPrices)
                .WithOne(p => p.SeatClass)
                .HasForeignKey(p => p.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
