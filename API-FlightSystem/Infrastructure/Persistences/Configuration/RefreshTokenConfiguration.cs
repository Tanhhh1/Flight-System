using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistences.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .HasMaxLength(500) 
                .IsRequired();

            builder.Property(x => x.JwtId)
                .HasMaxLength(150) 
                .IsRequired();

            builder.Property(x => x.ExpiryTime)
                .IsRequired();

            builder.Property(x => x.InRevoked)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens) 
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Token)
                .IsUnique()
                .HasDatabaseName("UX_RefreshToken_Token");
        }
    }
}
