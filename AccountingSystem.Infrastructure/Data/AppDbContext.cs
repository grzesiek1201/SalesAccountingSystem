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
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<NumberSequence> NumberSequences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================= PRODUCT =================
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Property(p => p.CategoryId)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            // ================= PRODUCT CATEGORY =================
            modelBuilder.Entity<ProductCategory>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<ProductCategory>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(64);

            modelBuilder.Entity<ProductCategory>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // ================= QUOTATION =================
            modelBuilder.Entity<Quotation>()
                .HasMany(q => q.Items)
                .WithOne(i => i.Quotation)
                .HasForeignKey(i => i.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<QuotationItem>()
                .Property(q => q.DiscountPercent)
                .HasPrecision(5, 2);

            // ================= ORDER =================
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.DiscountPercent)
                .HasPrecision(5, 2);

            // ================= INVOICE =================
            modelBuilder.Entity<Invoice>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Invoice)
                .HasForeignKey(i => i.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.BaseUnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.DiscountPercent)
                .HasPrecision(5, 2);
        }
    }
}