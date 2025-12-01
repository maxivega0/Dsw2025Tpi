using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data;

public class Dsw2025TpiContext : DbContext
{
    public Dsw2025TpiContext(DbContextOptions<Dsw2025TpiContext> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(eb =>
        {
            eb.ToTable("Products");
            eb.Property(p => p.Sku)
            .HasMaxLength(20)
            .IsRequired();
            eb.Property(p => p.InternalCode);
            eb.Property(p => p.Name)
            .HasMaxLength(60);
            eb.Property(p => p.Description);
            eb.Property(p => p.CurrentUnitPrice)
            .HasPrecision(15, 2)
            .IsRequired();
            eb.Property(p => p.StockQuantity)
            .HasConversion<int>()
            .IsRequired();
            eb.Property(p => p.Image)
            .HasMaxLength(250)
            .IsRequired();

        });
        
        modelBuilder.Entity<Order>(eb => {
            eb.ToTable("Orders");
            eb.Property(o => o.Date).HasColumnType("datetime2(7)").IsRequired();
            eb.Property(o => o.Status).IsRequired();
            //eb.Property(o => o.ShippingAddress).HasMaxLength(120).IsRequired();
            //eb.Property(o => o.BillingAddress).HasMaxLength(120).IsRequired();
            eb.Property(o => o.Notes).HasMaxLength(350);
        });

        modelBuilder.Entity<OrderItem>(eb => {
            eb.ToTable("OrderItems");
            eb.Property(oi => oi.Quantity).IsRequired();
            eb.Property(oi => oi.UnitPrice).HasPrecision(15, 2).IsRequired();
        });
    }
}
