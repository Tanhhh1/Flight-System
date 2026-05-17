using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SupportRequestConfiguration : IEntityTypeConfiguration<SupportRequest>
    {
        public void Configure(EntityTypeBuilder<SupportRequest> builder)
        {
            builder.HasKey(x => x.RequestId);

            builder.Property(x => x.Reason)
                .HasMaxLength(1000) 
                .IsRequired();

            builder.Property(x => x.RequestType)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.NewFlight)
                .WithMany()
                .HasForeignKey(x => x.NewFlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Booking)
                .WithMany(b => b.SupportRequests)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Status)
                .HasDatabaseName("IX_SupportRequest_Status");
        }
    }
}