namespace ReactiveOrders.ReadModel;

public class OrderListView
{
    public Guid OrderId { get; set; }
    public string AttendeeId { get; set; } = string.Empty;
    public string Status { get; set; } = "Placed";
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? EstimatedReady { get; set; }
    public string? RejectionReason { get; set; }
}

public class KitchenDashboardView
{
    public Guid OrderId { get; set; }
    public string AttendeeId { get; set; } = string.Empty;
    public string Status { get; set; } = "Placed";
    public List<string> Items { get; set; } = [];
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderStatsView
{
    public int Id { get; set; } = 1;
    public int TotalOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int RejectedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class PopularItemView
{
    public string ItemName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int TotalQuantity { get; set; }
}

public class OrderItemView
{
    public Guid OrderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
