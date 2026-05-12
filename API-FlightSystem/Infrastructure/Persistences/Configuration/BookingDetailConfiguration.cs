using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class BookingDetailConfiguration : IEntityTypeConfiguration<BookingDetail>
    {
        public void Configure(EntityTypeBuilder<BookingDetail> builder)
        {
            builder.HasKey(x => x.BookingDetailId);

            builder.Property(x => x.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.HasOne(x => x.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Flight)
                .WithMany(f => f.BookingDetails)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Passenger)
                .WithMany(p => p.BookingDetails)
                .HasForeignKey(x => x.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FlightSeat)
                .WithOne(s => s.BookingDetail)
                .HasForeignKey<BookingDetail>(x => x.FlightSeatId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.FlightId)
                .HasDatabaseName("IX_BookingDetail_Flight");
        }
    }
}
