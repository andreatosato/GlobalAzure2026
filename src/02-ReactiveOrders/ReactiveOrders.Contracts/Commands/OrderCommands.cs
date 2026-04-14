namespace ReactiveOrders.Contracts.Commands;

public record PlaceOrder(List<OrderItem> Items, string AttendeeId, string? Notes);
public record ConfirmOrder(Guid OrderId);
public record RejectOrder(Guid OrderId, string Reason);
public record CancelOrder(Guid OrderId);

public record OrderItem(string Name, int Quantity, decimal UnitPrice);
