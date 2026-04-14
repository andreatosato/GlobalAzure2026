using Microsoft.EntityFrameworkCore;
using ReactiveOrders.ReadModel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<ReadModelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("readdb")));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/api/orders", async (ReadModelDbContext db) =>
    await db.OrderList.OrderByDescending(o => o.CreatedAt).Take(50).ToListAsync());

app.MapGet("/api/orders/{orderId:guid}", async (Guid orderId, ReadModelDbContext db) =>
    await db.OrderList.FindAsync(orderId) is { } order
        ? Results.Ok(order)
        : Results.NotFound());

app.MapGet("/api/kitchen", async (ReadModelDbContext db) =>
    await db.KitchenDashboard
        .Where(k => k.Status == "Placed" || k.Status == "Confirmed")
        .OrderBy(k => k.CreatedAt)
        .ToListAsync());

app.MapGet("/api/stats", async (ReadModelDbContext db) =>
    await db.OrderStats.FindAsync(1) ?? new OrderStatsView());

app.MapGet("/api/popular-items", async (ReadModelDbContext db) =>
    await db.PopularItems.OrderByDescending(p => p.TotalQuantity).Take(10).ToListAsync());

app.Run();
