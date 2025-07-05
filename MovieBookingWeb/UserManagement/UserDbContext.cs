using Microsoft.EntityFrameworkCore;
using UserManagement.Models.Entities;
using UserManagement.Models.Entities;

namespace UserManagement
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=UserDB;Username=postgres;Password=12345");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    FullName = "Admin User",
                    BirthDate = new DateTime(1990, 1, 1),
                    Gender = "Nam",
                    Email = "admin@example.com",
                    IdCard = "0123456789",
                    PhoneNumber = "0901234567",
                    Address = "Admin Street",
                    Role = "Admin",
                    IsLocked = false
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    Password = "user123",
                    FullName = "Normal User",
                    BirthDate = new DateTime(2000, 12, 12),
                    Gender = "Nam",
                    Email = "user@example.com",
                    IdCard = "1122334455",
                    PhoneNumber = "0911223344",
                    Address = "User Street",
                    Role = "User",
                    IsLocked = false
                }
            );
        }
    }
}