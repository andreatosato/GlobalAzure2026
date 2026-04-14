using Microsoft.AspNetCore.SignalR;

namespace ReactiveOrders.Web.Hubs;

public class OrderHub : Hub
{
    public async Task OrderUpdate(string eventType, Guid orderId)
    {
        await Clients.All.SendAsync("OrderUpdate", eventType, orderId);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
