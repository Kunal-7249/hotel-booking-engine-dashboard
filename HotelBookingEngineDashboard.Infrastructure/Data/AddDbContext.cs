using HotelBookingEngineDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingEngineDashboard.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Hotel> Hotels => Set<Hotel>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Username)
                  .IsRequired()
                  .HasMaxLength(100);

                entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(200);

                entity.Property(u => u.PasswordHash)
                  .IsRequired();

                entity.Property(u => u.Role)
                  .HasConversion<string>()
                  .HasMaxLength(20);

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.HotelId);

                entity.Property(h => h.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(h => h.City)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(h => h.PricePerNight)
                      .HasColumnType("decimal(10,2)");

                entity.Property(h => h.StarRating)
                      .IsRequired();
            });

            //  Reservation config 
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => r.ReservationId);

                entity.Property(r => r.BookingRef)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(r => r.GuestName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(r => r.Status)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(r => r.Hotel)
                      .WithMany(h => h.Reservations)
                      .HasForeignKey(r => r.HotelId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });



            // Seed Hotels
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { HotelId = 1, Name = "The Grand Oberoi", City = "Mumbai", StarRating = 5, PricePerNight = 12000, Description = "Luxury hotel in the heart of Mumbai.", IsAvailable = true },
                new Hotel { HotelId = 2, Name = "Lemon Tree Premier", City = "Pune", StarRating = 4, PricePerNight = 5500, Description = "Modern business hotel in Pune.", IsAvailable = true },
                new Hotel { HotelId = 3, Name = "Taj Lake Palace", City = "Udaipur", StarRating = 5, PricePerNight = 25000, Description = "Iconic heritage palace on Lake Pichola.", IsAvailable = true },
                new Hotel { HotelId = 4, Name = "Ibis Styles", City = "Bangalore", StarRating = 3, PricePerNight = 3200, Description = "Affordable comfort near tech parks.", IsAvailable = true },
                new Hotel { HotelId = 5, Name = "Novotel Juhu Beach", City = "Mumbai", StarRating = 4, PricePerNight = 8500, Description = "Beachside hotel with sea views.", IsAvailable = true },
                new Hotel { HotelId = 6, Name = "The Lalit", City = "Delhi", StarRating = 5, PricePerNight = 15000, Description = "Grand luxury hotel in New Delhi.", IsAvailable = true },
                new Hotel { HotelId = 7, Name = "ITC Grand Chola", City = "Chennai", StarRating = 5, PricePerNight = 18000, Description = "Iconic luxury hotel inspired by Chola architecture.", IsAvailable = true },
                new Hotel { HotelId = 8, Name = "Marriott", City = "Pune", StarRating = 5, PricePerNight = 9000, Description = "Premium business hotel in Pune.", IsAvailable = true },
                new Hotel { HotelId = 9, Name = "Hyatt Regency", City = "Mumbai", StarRating = 5, PricePerNight = 14000, Description = "Luxury hotel with stunning city views.", IsAvailable = true },
                new Hotel { HotelId = 10, Name = "Radisson Blu", City = "Delhi", StarRating = 4, PricePerNight = 7500, Description = "Modern hotel near business district.", IsAvailable = true },
                new Hotel { HotelId = 11, Name = "Fortune Select", City = "Bangalore", StarRating = 4, PricePerNight = 4500, Description = "Comfortable stay in tech city.", IsAvailable = true },
                new Hotel { HotelId = 12, Name = "Trident Hotel", City = "Udaipur", StarRating = 5, PricePerNight = 20000, Description = "Lakeside luxury with heritage charm.", IsAvailable = true }
            );
        }
    }
}
