using ReactiveOrders.Contracts.Commands;
using ReactiveOrders.Contracts.Events;
using ReactiveOrders.Domain;

namespace ReactiveOrders.CommandApi.Handlers;

public static class PlaceOrderHandler
{
    public static async Task<(Guid, OrderPlaced)> HandleAsync(PlaceOrder command, OrderDbContext db)
    {
        var order = Order.Create(command);
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var @event = new OrderPlaced(
            order.Id,
            command.Items.Select(i => new OrderItemInfo(i.Name, i.Quantity, i.UnitPrice)).ToList(),
            command.AttendeeId,
            DateTime.UtcNow);

        return (order.Id, @event);
    }
}

public static class ConfirmOrderHandler
{
    public static async Task<OrderConfirmed> HandleAsync(ConfirmOrder command, OrderDbContext db)
    {
        var order = await db.Orders.FindAsync(command.OrderId)
            ?? throw new InvalidOperationException($"Order {command.OrderId} not found");

        var estimatedReady = DateTime.UtcNow.AddSeconds(30);
        order.Confirm(estimatedReady);
        await db.SaveChangesAsync();

        return new OrderConfirmed(order.Id, estimatedReady, DateTime.UtcNow);
    }
}

public static class RejectOrderHandler
{
    public static async Task<OrderRejected> HandleAsync(RejectOrder command, OrderDbContext db)
    {
        var order = await db.Orders.FindAsync(command.OrderId)
            ?? throw new InvalidOperationException($"Order {command.OrderId} not found");

        order.Reject(command.Reason);
        await db.SaveChangesAsync();

        return new OrderRejected(order.Id, command.Reason, DateTime.UtcNow);
    }
}

public static class CancelOrderHandler
{
    public static async Task<OrderCancelled> HandleAsync(CancelOrder command, OrderDbContext db)
    {
        var order = await db.Orders.FindAsync(command.OrderId)
            ?? throw new InvalidOperationException($"Order {command.OrderId} not found");

        order.Cancel();
        await db.SaveChangesAsync();

        return new OrderCancelled(order.Id, DateTime.UtcNow);
    }
}
