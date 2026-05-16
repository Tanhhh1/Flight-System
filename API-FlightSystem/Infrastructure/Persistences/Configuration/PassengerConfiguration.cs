using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(x => x.PassengerId);

            builder.Property(x => x.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Gender)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Birthday)
                .HasColumnType("date")
                .IsRequired();

            builder.HasOne(x => x.PassengerType)
                .WithMany(t => t.Passengers)
                .HasForeignKey(x => x.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.BookingDetails)
                .WithOne(d => d.Passenger)
                .HasForeignKey(d => d.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
