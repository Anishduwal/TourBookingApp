using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourBooking.Domain.Entities;

namespace TourBooking.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Tour> Tours => Set<Tour>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Email).HasMaxLength(254).IsRequired();
                entity.Property(u => u.NormalizedEmail).HasMaxLength(254).IsRequired();
                entity.HasIndex(u => u.NormalizedEmail).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
                entity.HasIndex(t => t.TokenHash).IsUnique();
                entity.Property(t => t.RowVersion).IsRowVersion();
                entity.HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);
            });
        }

    }
}
