using backend.models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<PurchaseOrders> PurchaseOrders { get; set; }
        public DbSet<PurchaseLines> PurchaseLines { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderLines> OrderLines { get; set; }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<InvoiceLines> InvoiceLines { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Departments> Departments { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Roles>()
                .Property(r => r.Permissions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new()
                );

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Customer).WithMany(c => c.Orders)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoices>()
                .HasOne(i => i.Customer).WithMany(c => c.Invoices)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoices>()
                .HasOne(i => i.Order).WithOne(o => o.Invoice)
                .HasForeignKey<Invoices>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrders>()
                .HasOne(p => p.Supplier).WithMany(s => s.PurchaseOrders)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Category).WithMany(c => c.Products)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employees>()
                .HasOne(e => e.Department).WithMany(d => d.Employees)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role).WithMany(r => r.Users)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User).WithMany(u => u.Tokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderLines>()
                .HasOne(ol => ol.Order).WithMany(o => o.Lines)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderLines>()
                .HasOne(ol => ol.Product).WithMany(p => p.OrderLines)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseLines>()
                .HasOne(pl => pl.PurchaseOrder).WithMany(po => po.Lines)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseLines>()
                .HasOne(pl => pl.Product).WithMany(p => p.PurchaseLines)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InvoiceLines>()
                .HasOne(il => il.Invoice).WithMany(i => i.Lines)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
