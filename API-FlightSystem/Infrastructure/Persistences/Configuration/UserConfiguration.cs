using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.Id).HasColumnName("UserId");
            builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(100);
            builder.Property(u => u.PasswordHash).HasColumnName("Password");
            builder.Property(u => u.PhoneNumber).HasColumnName("Phone");

            builder.Property(u => u.Fullname)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.Address)
                .HasMaxLength(250);

            builder.Property(u => u.Gender)
                .HasMaxLength(10);

            builder.Property(u => u.Birthday)
                .HasColumnType("date");

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}