using Microsoft.EntityFrameworkCore;

namespace ReactiveOrders.ReadModel;

public class ReadModelDbContext : DbContext
{
    public ReadModelDbContext(DbContextOptions<ReadModelDbContext> options) : base(options) { }

    public DbSet<OrderListView> OrderList => Set<OrderListView>();
    public DbSet<KitchenDashboardView> KitchenDashboard => Set<KitchenDashboardView>();
    public DbSet<OrderStatsView> OrderStats => Set<OrderStatsView>();
    public DbSet<PopularItemView> PopularItems => Set<PopularItemView>();
    public DbSet<OrderItemView> OrderItems => Set<OrderItemView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderListView>(e =>
        {
            e.HasKey(o => o.OrderId);
            e.ToTable("OrderListView");
        });

        modelBuilder.Entity<KitchenDashboardView>(e =>
        {
            e.HasKey(o => o.OrderId);
            e.ToTable("KitchenDashboardView");
            e.Ignore(o => o.Items);
        });

        modelBuilder.Entity<OrderStatsView>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).ValueGeneratedNever();
            e.ToTable("OrderStatsView");
        });

        modelBuilder.Entity<PopularItemView>(e =>
        {
            e.HasKey(o => o.ItemName);
            e.Property(o => o.ItemName).HasMaxLength(200);
            e.ToTable("PopularItemView");
        });

        modelBuilder.Entity<OrderItemView>(e =>
        {
            e.HasKey(o => new { o.OrderId, o.Name });
            e.Property(o => o.Name).HasMaxLength(200);
            e.ToTable("OrderItemView");
        });
    }
}
