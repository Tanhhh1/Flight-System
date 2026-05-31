using Domain.Common;
using Domain.Entities;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Infrastructure.Persistences
{
    public class DatabaseContext : IdentityDbContext<
      User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Airline> Airlines => Set<Airline>();
        public DbSet<Airport> Airports => Set<Airport>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingDetail> BookingDetails => Set<BookingDetail>();
        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<FlightSeat> FlightSeats => Set<FlightSeat>();
        public DbSet<FlightSeatPrice> FlightSeatPrices => Set<FlightSeatPrice>();
        public DbSet<FlightSegment> FlightSegments => Set<FlightSegment>();
        public DbSet<FlightService> FlightServices => Set<FlightService>();
        public DbSet<Passenger> Passengers => Set<Passenger>();
        public DbSet<PassengerType> PassengerTypes => Set<PassengerType>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Plane> Planes => Set<Plane>();
        public DbSet<Policy> Policies => Set<Policy>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<SeatClass> SeatClasses => Set<SeatClass>();
        public DbSet<SeatTemplate> SeatTemplates => Set<SeatTemplate>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
              Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}