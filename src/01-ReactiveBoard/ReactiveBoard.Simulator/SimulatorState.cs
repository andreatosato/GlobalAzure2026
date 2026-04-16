using Microsoft.Extensions.Logging;

namespace ReactiveBoard.Simulator;

/// <summary>
/// Observable state shared between the background simulator and the Blazor UI.
/// </summary>
public sealed class SimulatorState(ILogger<SimulatorState> logger)
{
    public string CurrentTalk { get; private set; } = string.Empty;
    public string NextTalk { get; private set; } = string.Empty;
    public int RemainingSeconds { get; private set; }
    public int ProgressPercent { get; private set; }
    public int AttendeeCount { get; private set; }
    public string LastAttendeeAction { get; private set; } = string.Empty;
    public int TotalVotes { get; private set; }
    public string LastRating { get; private set; } = string.Empty;
    public bool IsConnected { get; private set; }
    public int EventsSent { get; private set; }

    public event Action? OnChange;

    public void UpdateTalk(string current, string next, int remaining, int progress)
    {
        CurrentTalk = current;
        NextTalk = next;
        RemainingSeconds = remaining;
        ProgressPercent = progress;
        EventsSent++;
        logger.LogInformation("🎤 Talk: {Talk} | ⏱️ {Remaining}s | {Progress}%", current, remaining, progress);
        OnChange?.Invoke();
    }

    public void UpdateAttendees(int count, string action)
    {
        AttendeeCount = count;
        LastAttendeeAction = action;
        EventsSent++;
        logger.LogInformation("👥 Attendees: {Count} ({Action})", count, action);
        OnChange?.Invoke();
    }

    public void UpdateVote(string rating, int totalVotes)
    {
        LastRating = rating;
        TotalVotes = totalVotes;
        EventsSent++;
        logger.LogInformation("🗳️ Vote: {Rating} | Total: {Total}", rating, totalVotes);
        OnChange?.Invoke();
    }

    public void SetConnected(bool connected)
    {
        IsConnected = connected;
        logger.LogInformation("📡 Connection: {Status}", connected ? "Connected" : "Disconnected");
        OnChange?.Invoke();
    }
}
