using Microsoft.EntityFrameworkCore;
using ReactiveOrders.Contracts.Commands;
using ReactiveOrders.Domain;
using Wolverine;
using Wolverine.AzureServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ordersdb")));

builder.UseWolverine(opts =>
{
    opts.UseAzureServiceBus(builder.Configuration.GetConnectionString("messaging")!)
        .AutoProvision();

    opts.PublishMessage<ReactiveOrders.Contracts.Events.OrderPlaced>()
        .ToAzureServiceBusTopic("order-events");
    opts.PublishMessage<ReactiveOrders.Contracts.Events.OrderConfirmed>()
        .ToAzureServiceBusTopic("order-events");
    opts.PublishMessage<ReactiveOrders.Contracts.Events.OrderRejected>()
        .ToAzureServiceBusTopic("order-events");
    opts.PublishMessage<ReactiveOrders.Contracts.Events.OrderCancelled>()
        .ToAzureServiceBusTopic("order-events");

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Ensure DB is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.MapPost("/api/orders", async (PlaceOrder command, IMessageBus bus) =>
{
    var result = await bus.InvokeAsync<Guid>(command);
    return Results.Created($"/api/orders/{result}", new { OrderId = result });
});

app.MapPost("/api/orders/{orderId:guid}/confirm", async (Guid orderId, IMessageBus bus) =>
{
    await bus.InvokeAsync(new ConfirmOrder(orderId));
    return Results.Ok();
});

app.MapPost("/api/orders/{orderId:guid}/reject", async (Guid orderId, RejectRequest request, IMessageBus bus) =>
{
    await bus.InvokeAsync(new RejectOrder(orderId, request.Reason));
    return Results.Ok();
});

app.MapPost("/api/orders/{orderId:guid}/cancel", async (Guid orderId, IMessageBus bus) =>
{
    await bus.InvokeAsync(new CancelOrder(orderId));
    return Results.Ok();
});

app.Run();

public record RejectRequest(string Reason);
