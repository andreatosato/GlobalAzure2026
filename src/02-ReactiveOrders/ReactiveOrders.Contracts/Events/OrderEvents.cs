namespace ReactiveOrders.Contracts.Events;

public record OrderPlaced(Guid OrderId, List<OrderItemInfo> Items, string AttendeeId, DateTime Timestamp);
public record OrderConfirmed(Guid OrderId, DateTime EstimatedReady, DateTime Timestamp);
public record OrderRejected(Guid OrderId, string Reason, DateTime Timestamp);
public record OrderCancelled(Guid OrderId, DateTime Timestamp);

public record OrderItemInfo(string Name, int Quantity, decimal UnitPrice);
