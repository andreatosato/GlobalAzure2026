using Microsoft.EntityFrameworkCore;
using ReactiveOrders.Domain;

namespace ReactiveOrders.Domain;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.AttendeeId).HasMaxLength(100);
            e.Property(o => o.Notes).HasMaxLength(500);
            e.Property(o => o.RejectionReason).HasMaxLength(500);
            e.HasMany(o => o.Lines).WithOne().HasForeignKey("OrderId");
        });

        modelBuilder.Entity<OrderLine>(e =>
        {
            e.HasKey("OrderId", "Name");
            e.Property(l => l.Name).HasMaxLength(200);
        });
    }
}
