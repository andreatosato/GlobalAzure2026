using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReactiveOrders.Contracts.Events;
using ReactiveOrders.ReadModel;

namespace ReactiveOrders.EventProcessor.Handlers;

public class OrderPlacedHandler(ReadModelDbContext db, HubConnection hub, ILogger<OrderPlacedHandler> logger)
{
    public async Task Handle(OrderPlaced @event)
    {
        logger.LogInformation("Processing OrderPlaced: {OrderId}", @event.OrderId);

        // Update OrderListView
        db.OrderList.Add(new OrderListView
        {
            OrderId = @event.OrderId,
            AttendeeId = @event.AttendeeId,
            Status = "Placed",
            TotalAmount = @event.Items.Sum(i => i.Quantity * i.UnitPrice),
            ItemCount = @event.Items.Sum(i => i.Quantity),
            CreatedAt = @event.Timestamp
        });

        // Update KitchenDashboard
        db.KitchenDashboard.Add(new KitchenDashboardView
        {
            OrderId = @event.OrderId,
            AttendeeId = @event.AttendeeId,
            Status = "Placed",
            CreatedAt = @event.Timestamp
        });

        // Update Stats
        var stats = await db.OrderStats.FindAsync(1);
        if (stats is null)
        {
            stats = new OrderStatsView { TotalOrders = 1, PendingOrders = 1, LastUpdated = DateTime.UtcNow };
            db.OrderStats.Add(stats);
        }
        else
        {
            stats.TotalOrders++;
            stats.PendingOrders++;
            stats.LastUpdated = DateTime.UtcNow;
        }

        // Update PopularItems
        foreach (var item in @event.Items)
        {
            var popular = await db.PopularItems.FindAsync(item.Name);
            if (popular is null)
            {
                db.PopularItems.Add(new PopularItemView
                {
                    ItemName = item.Name,
                    OrderCount = 1,
                    TotalQuantity = item.Quantity
                });
            }
            else
            {
                popular.OrderCount++;
                popular.TotalQuantity += item.Quantity;
            }
        }

        await db.SaveChangesAsync();

        // Push via SignalR
        if (hub.State == HubConnectionState.Connected)
        {
            await hub.InvokeAsync("OrderUpdate", "OrderPlaced", @event.OrderId);
        }
    }
}

public class OrderConfirmedHandler(ReadModelDbContext db, HubConnection hub, ILogger<OrderConfirmedHandler> logger)
{
    public async Task Handle(OrderConfirmed @event)
    {
        logger.LogInformation("Processing OrderConfirmed: {OrderId}", @event.OrderId);

        var orderView = await db.OrderList.FindAsync(@event.OrderId);
        if (orderView is not null)
        {
            orderView.Status = "Confirmed";
            orderView.ConfirmedAt = @event.Timestamp;
            orderView.EstimatedReady = @event.EstimatedReady;
        }

        var kitchenView = await db.KitchenDashboard.FindAsync(@event.OrderId);
        if (kitchenView is not null)
        {
            kitchenView.Status = "Confirmed";
        }

        var stats = await db.OrderStats.FindAsync(1);
        if (stats is not null)
        {
            stats.ConfirmedOrders++;
            stats.PendingOrders = Math.Max(0, stats.PendingOrders - 1);
            stats.TotalRevenue += orderView?.TotalAmount ?? 0;
            stats.LastUpdated = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        if (hub.State == HubConnectionState.Connected)
        {
            await hub.InvokeAsync("OrderUpdate", "OrderConfirmed", @event.OrderId);
        }
    }
}

public class OrderRejectedHandler(ReadModelDbContext db, HubConnection hub, ILogger<OrderRejectedHandler> logger)
{
    public async Task Handle(OrderRejected @event)
    {
        logger.LogInformation("Processing OrderRejected: {OrderId}", @event.OrderId);

        var orderView = await db.OrderList.FindAsync(@event.OrderId);
        if (orderView is not null)
        {
            orderView.Status = "Rejected";
            orderView.RejectionReason = @event.Reason;
        }

        var kitchenView = await db.KitchenDashboard.FindAsync(@event.OrderId);
        if (kitchenView is not null)
        {
            db.KitchenDashboard.Remove(kitchenView);
        }

        var stats = await db.OrderStats.FindAsync(1);
        if (stats is not null)
        {
            stats.RejectedOrders++;
            stats.PendingOrders = Math.Max(0, stats.PendingOrders - 1);
            stats.LastUpdated = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        if (hub.State == HubConnectionState.Connected)
        {
            await hub.InvokeAsync("OrderUpdate", "OrderRejected", @event.OrderId);
        }
    }
}

public class OrderCancelledHandler(ReadModelDbContext db, HubConnection hub, ILogger<OrderCancelledHandler> logger)
{
    public async Task Handle(OrderCancelled @event)
    {
        logger.LogInformation("Processing OrderCancelled: {OrderId}", @event.OrderId);

        var orderView = await db.OrderList.FindAsync(@event.OrderId);
        if (orderView is not null)
        {
            orderView.Status = "Cancelled";
        }

        var kitchenView = await db.KitchenDashboard.FindAsync(@event.OrderId);
        if (kitchenView is not null)
        {
            db.KitchenDashboard.Remove(kitchenView);
        }

        var stats = await db.OrderStats.FindAsync(1);
        if (stats is not null)
        {
            stats.CancelledOrders++;
            stats.PendingOrders = Math.Max(0, stats.PendingOrders - 1);
            stats.LastUpdated = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        if (hub.State == HubConnectionState.Connected)
        {
            await hub.InvokeAsync("OrderUpdate", "OrderCancelled", @event.OrderId);
        }
    }
}
