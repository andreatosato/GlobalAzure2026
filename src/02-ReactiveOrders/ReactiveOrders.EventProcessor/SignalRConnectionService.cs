using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReactiveOrders.EventProcessor;

public class SignalRConnectionService(HubConnection connection, ILogger<SignalRConnectionService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (connection.State == HubConnectionState.Disconnected)
                {
                    await connection.StartAsync(stoppingToken);
                    logger.LogInformation("Connected to OrderHub");
                }
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to connect to OrderHub, retrying in 3s...");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
