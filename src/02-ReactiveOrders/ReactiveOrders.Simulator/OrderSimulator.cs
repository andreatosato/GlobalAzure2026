using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveOrders.Contracts.Commands;

namespace ReactiveOrders.Simulator;

public class OrderSimulator : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrderSimulator> _logger;
    private readonly Random _random = new();

    private static readonly (string Name, decimal Price)[] Menu =
    [
        ("Margherita Pizza", 8.50m),
        ("Pepperoni Pizza", 10.00m),
        ("Caesar Salad", 7.00m),
        ("Carbonara Pasta", 9.50m),
        ("Tiramisu", 6.00m),
        ("Bruschetta", 5.50m),
        ("Risotto ai Funghi", 11.00m),
        ("Gelato", 4.50m),
        ("Espresso", 2.50m),
        ("Acqua Minerale", 1.50m)
    ];

    private static readonly string[] Attendees =
    [
        "attendee-001", "attendee-002", "attendee-003", "attendee-004",
        "attendee-005", "attendee-006", "attendee-007", "attendee-008"
    ];

    public OrderSimulator(IHttpClientFactory httpClientFactory, ILogger<OrderSimulator> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderSimulator starting...");
        await Task.Delay(3000, stoppingToken);

        var placedOrders = new List<Guid>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Action mix: 40% place · 45% confirm (oldest first) · 8% reject · 7% cancel
                var action = placedOrders.Count > 0 ? _random.Next(0, 100) : 0;

                if (action < 40)
                {
                    var orderId = await PlaceOrderAsync(stoppingToken);
                    if (orderId.HasValue)
                        placedOrders.Add(orderId.Value);
                }
                else if (action < 85)
                {
                    // Confirm the oldest pending order (FIFO, like a real kitchen)
                    var orderId = placedOrders[0];
                    placedOrders.RemoveAt(0);
                    await ConfirmOrderAsync(orderId, stoppingToken);
                }
                else if (action < 93)
                {
                    var orderId = placedOrders[_random.Next(placedOrders.Count)];
                    placedOrders.Remove(orderId);
                    await RejectOrderAsync(orderId, stoppingToken);
                }
                else
                {
                    var orderId = placedOrders[_random.Next(placedOrders.Count)];
                    placedOrders.Remove(orderId);
                    await CancelOrderAsync(orderId, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Simulator error, retrying...");
            }

            await Task.Delay(TimeSpan.FromSeconds(_random.Next(2, 5)), stoppingToken);
        }
    }

    private async Task<Guid?> PlaceOrderAsync(CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("commandapi");
        var itemCount = _random.Next(1, 4);
        var picked = new Dictionary<string, (int Quantity, decimal Price)>();
        for (int i = 0; i < itemCount; i++)
        {
            var (name, price) = Menu[_random.Next(Menu.Length)];
            var qty = _random.Next(1, 3);
            if (picked.TryGetValue(name, out var existing))
                picked[name] = (existing.Quantity + qty, price);
            else
                picked[name] = (qty, price);
        }
        var items = picked.Select(p => new OrderItem(p.Key, p.Value.Quantity, p.Value.Price)).ToList();

        var command = new PlaceOrder(items, Attendees[_random.Next(Attendees.Length)], $"Simulated order");
        var response = await client.PostAsJsonAsync("/api/orders", command, ct);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<OrderResult>(ct);
            _logger.LogInformation("Placed order {OrderId}", result?.OrderId);
            return result?.OrderId;
        }

        _logger.LogWarning("Failed to place order: {Status}", response.StatusCode);
        return null;
    }

    private async Task ConfirmOrderAsync(Guid orderId, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("commandapi");
        var response = await client.PostAsync($"/api/orders/{orderId}/confirm", null, ct);
        _logger.LogInformation("Confirmed order {OrderId}: {Status}", orderId, response.StatusCode);
    }

    private async Task RejectOrderAsync(Guid orderId, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("commandapi");
        var reasons = new[] { "Out of stock", "Kitchen closed", "Item unavailable" };
        var reason = reasons[_random.Next(reasons.Length)];
        var response = await client.PostAsJsonAsync($"/api/orders/{orderId}/reject", new { Reason = reason }, ct);
        _logger.LogInformation("Rejected order {OrderId}: {Status}", orderId, response.StatusCode);
    }

    private async Task CancelOrderAsync(Guid orderId, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("commandapi");
        var response = await client.PostAsync($"/api/orders/{orderId}/cancel", null, ct);
        _logger.LogInformation("Cancelled order {OrderId}: {Status}", orderId, response.StatusCode);
    }

    private record OrderResult(Guid OrderId);
}
