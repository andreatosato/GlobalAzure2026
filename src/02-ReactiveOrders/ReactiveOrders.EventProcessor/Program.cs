using Azure.Identity;
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
    // L'emulatore Azure Service Bus non supporta l'API HTTP di amministrazione
    // né la creazione dinamica di queue di sistema (retries/response).
    // Topic/subscription sono preconfigurati dall'AppHost.
    var messagingConn = builder.Configuration.GetConnectionString("messaging")!;
    if (messagingConn.Contains("SharedAccessKey", StringComparison.OrdinalIgnoreCase)
        || messagingConn.Contains("Endpoint=", StringComparison.OrdinalIgnoreCase))
    {
        opts.UseAzureServiceBus(messagingConn).SystemQueuesAreEnabled(false);
    }
    else
    {
        opts.UseAzureServiceBus(messagingConn, new DefaultAzureCredential())
            .SystemQueuesAreEnabled(false);
    }

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
