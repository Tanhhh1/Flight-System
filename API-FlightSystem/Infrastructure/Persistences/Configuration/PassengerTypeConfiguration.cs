using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class PassengerTypeConfiguration : IEntityTypeConfiguration<PassengerType>
    {
        public void Configure(EntityTypeBuilder<PassengerType> builder)
        {
            builder.HasKey(x => x.TypeId);

            builder.Property(x => x.TypeName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.DiscountRate)
                .HasPrecision(5, 2) 
                .HasDefaultValue(0);

            builder.HasMany(x => x.Passengers)
                .WithOne(p => p.PassengerType)
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
