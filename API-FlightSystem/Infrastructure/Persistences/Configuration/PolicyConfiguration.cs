using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Persistences.Configuration
{
    public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.HasKey(x => x.PolicyId);

            builder.Property(x => x.IsRefund)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsChange)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasMany(x => x.Flights)
                .WithOne(f => f.Policy)
                .HasForeignKey(f => f.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);
        } 
    }
}
