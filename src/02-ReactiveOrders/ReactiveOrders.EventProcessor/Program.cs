using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveOrders.ReadModel;
using Wolverine;
using Wolverine.AzureServiceBus;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<ReadModelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("readdb")));

builder.Services.AddSingleton<HubConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var webUrl = config["services:web:http:0"]
                 ?? config["services:web:https:0"]
                 ?? "http://localhost:5000";
    return new HubConnectionBuilder()
        .WithUrl($"{webUrl}/orderhub")
        .WithAutomaticReconnect()
        .Build();
});

builder.Services.AddHostedService<ReactiveOrders.EventProcessor.SignalRConnectionService>();

builder.UseWolverine(opts =>
{
    opts.UseAzureServiceBus(builder.Configuration.GetConnectionString("messaging")!)
        .AutoProvision();

    opts.ListenToAzureServiceBusSubscription("event-processor")
        .FromTopic("order-events")
        .ProcessInline();

    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

var host = builder.Build();

// Ensure read model DB is created
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReadModelDbContext>();
    await db.Database.EnsureCreatedAsync();
}

host.Run();
