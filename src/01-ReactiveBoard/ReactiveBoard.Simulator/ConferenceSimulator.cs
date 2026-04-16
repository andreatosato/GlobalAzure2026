using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReactiveBoard.Simulator;

public class ConferenceSimulator(
    ILogger<ConferenceSimulator> logger,
    IConfiguration configuration,
    SimulatorState state) : BackgroundService
{
    private static readonly string[] Talks =
    [
        "I mille modi di creare un'app reattiva su Azure",
        "Kubernetes in produzione: lezioni apprese",
        "Blazor .NET 10: cosa c'è di nuovo",
        "AI + Azure: dall'idea al deploy",
        "Microservizi con Aspire e Service Bus",
        "DevOps con GitHub Actions e Azure"
    ];

    private static readonly string[] Ratings = ["⭐", "⭐⭐", "⭐⭐⭐", "⭐⭐⭐⭐", "⭐⭐⭐⭐⭐"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var webUrl = configuration["services:web:http:0"]
                     ?? configuration["services:web:https:0"]
                     ?? "http://localhost:5000";

        logger.LogInformation("Simulator starting, connecting to {Url}/conferencehub", webUrl);

        HubConnection? connection = null;

        // Retry connection loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl($"{webUrl}/conferencehub")
                    .WithAutomaticReconnect()
                    .Build();

                await connection.StartAsync(stoppingToken);
                logger.LogInformation("Connected to ConferenceHub");
                state.SetConnected(true);
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to connect to hub, retrying in 3s...");
                if (connection is not null)
                {
                    await connection.DisposeAsync();
                    connection = null;
                }
                await Task.Delay(3000, stoppingToken);
            }
        }

        if (connection is null || stoppingToken.IsCancellationRequested)
            return;

        var random = new Random();
        var attendeeCount = 50;
        var currentTalkIndex = 0;
        var talkStartTime = DateTime.UtcNow;
        var talkDuration = TimeSpan.FromSeconds(60); // 60s per talk for demo speed
        var totalVotes = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var elapsed = DateTime.UtcNow - talkStartTime;
                if (elapsed > talkDuration)
                {
                    currentTalkIndex = (currentTalkIndex + 1) % Talks.Length;
                    talkStartTime = DateTime.UtcNow;
                    elapsed = TimeSpan.Zero;
                    logger.LogInformation("Talk changed to: {Talk}", Talks[currentTalkIndex]);
                }

                var remaining = talkDuration - elapsed;
                var progressPercent = (int)(elapsed.TotalSeconds / talkDuration.TotalSeconds * 100);

                // Send talk update
                await connection.InvokeAsync("SendTalkUpdate",
                    Talks[currentTalkIndex],
                    Talks[(currentTalkIndex + 1) % Talks.Length],
                    (int)remaining.TotalSeconds,
                    progressPercent,
                    stoppingToken);

                state.UpdateTalk(Talks[currentTalkIndex], Talks[(currentTalkIndex + 1) % Talks.Length],
                    (int)remaining.TotalSeconds, progressPercent);

                // Simulate attendee check-in/out
                var action = random.Next(10) < 7 ? "check-in" : "check-out";
                attendeeCount += action == "check-in" ? random.Next(1, 5) : -random.Next(1, 3);
                attendeeCount = Math.Max(10, Math.Min(500, attendeeCount));

                await connection.InvokeAsync("SendAttendeeUpdate",
                    attendeeCount,
                    action == "check-in" ? $"+{random.Next(1, 5)} check-in" : $"-{random.Next(1, 3)} check-out",
                    stoppingToken);

                state.UpdateAttendees(attendeeCount,
                    action == "check-in" ? "check-in" : "check-out");

                // Simulate votes (70% chance each cycle)
                if (random.Next(10) < 7)
                {
                    var rating = Ratings[random.Next(Ratings.Length)];
                    totalVotes++;
                    await connection.InvokeAsync("SendVoteUpdate",
                        Talks[currentTalkIndex],
                        rating,
                        totalVotes,
                        stoppingToken);

                    state.UpdateVote(rating, totalVotes);
                }

                var delay = random.Next(2000, 4000);
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error sending update, will retry...");
                await Task.Delay(3000, stoppingToken);
            }
        }

        if (connection is not null)
        {
            state.SetConnected(false);
            await connection.DisposeAsync();
        }
    }
}
