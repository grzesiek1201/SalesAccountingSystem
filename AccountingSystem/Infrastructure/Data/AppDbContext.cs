using Microsoft.EntityFrameworkCore;
using AccountingSystem.Domain.Entities;

namespace AccountingSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.DiscountPercent)
                .HasPrecision(5, 2);

            modelBuilder.Entity<OrderItem>()
                 .Property(q => q.BaseUnitPrice)
                 .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(q => q.DiscountPercent)
                .HasPrecision(5, 2);
        }
    }
}