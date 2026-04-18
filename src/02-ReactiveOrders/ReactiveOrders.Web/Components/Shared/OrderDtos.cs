namespace ReactiveOrders.Web.Components.Shared;

public record OrderView(
    Guid OrderId,
    string AttendeeId,
    string Status,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt,
    DateTime? ConfirmedAt = null,
    DateTime? EstimatedReady = null,
    string? RejectionReason = null);

public record StatsView(
    int TotalOrders = 0,
    int ConfirmedOrders = 0,
    int RejectedOrders = 0,
    int CancelledOrders = 0,
    int PendingOrders = 0,
    decimal TotalRevenue = 0);

public record PopularItem(string ItemName, int OrderCount, int TotalQuantity);

public record OrderItemDto(string Name, int Quantity, decimal UnitPrice);

public record OrderDetails(OrderView Order, List<OrderItemDto> Items);
