using Microsoft.EntityFrameworkCore;
using RKM_Server.Models;
using System.Data.SqlClient;

namespace RKM_Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockAccount> StockAccounts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAccount> ProjectAccounts { get; set; }
        public DbSet<Orderer> Orderers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orderer>()
           .HasOne(b => b.Project)
           .WithOne(i => i.Orderer)
           .HasForeignKey<Project>(b => b.OrdererId);

            modelBuilder.Entity<Project>()
                        .HasOne(u => u.User)
                        .WithMany(u => u.Projects).
                        IsRequired().OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>()
                .HasKey(t => new { t.Id, t.UserId });

            modelBuilder.Entity<Project>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.Projects)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<StockAccount>()
                       .HasOne(u => u.User)
                       .WithMany(u => u.StockAccounts).
                       IsRequired().OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<StockAccount>()
                .HasKey(t => new { t.Id, t.UserId });

            modelBuilder.Entity<StockAccount>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.StockAccounts)
                .HasForeignKey(pt => pt.UserId);
        }
    }
}