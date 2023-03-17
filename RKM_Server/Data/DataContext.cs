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
            
        }
    }
}