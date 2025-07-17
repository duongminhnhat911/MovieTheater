using Microsoft.EntityFrameworkCore;
using BookingManagement.Models.Entities;

namespace BookingManagement
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SeatShowtime> SeatShowtimes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SeatShowtime>()
                .HasKey(ss => new { ss.SeatId, ss.ShowtimeId });

            modelBuilder.Entity<SeatShowtime>()
                .Property(ss => ss.Status)
                .HasConversion<string>();

            modelBuilder.Entity<SeatShowtime>()
                .HasOne(ss => ss.Seat)
                .WithMany()
                .HasForeignKey(ss => ss.SeatId);

            modelBuilder.Entity<SeatShowtime>()
                .HasOne(ss => ss.Showtime)
                .WithMany()
                .HasForeignKey(ss => ss.ShowtimeId);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Promotion)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PromotionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}