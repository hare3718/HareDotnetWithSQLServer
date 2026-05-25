using Microsoft.EntityFrameworkCore;
using hareDotnetSecondAPI.Models;
using hareDotnetSecondAPI.Data;

namespace hareDotnetSecondAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _configuration;
        public DataContextEF( IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<UserJobInfo> UserJobInfo { get; set; }
        public DbSet<UserSalary> UserSalary { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), options => options.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("HareDotnetFirstSchema");

            modelBuilder.Entity<Users>().ToTable("Users", "HareDotnetFirstSchema").HasKey(u => u.UserId);
            modelBuilder.Entity<UserJobInfo>().HasKey(u => u.UserId);
            modelBuilder.Entity<UserSalary>().HasKey(u => u.UserId);
        }
    }
}