using Microsoft.AspNetCore.SignalR;

namespace ReactiveBoard.Web.Hubs;

public class ConferenceHub : Hub
{
    public async Task SendAttendeeUpdate(int count, string action)
    {
        await Clients.All.SendAsync("AttendeeUpdate", count, action);
    }

    public async Task SendVoteUpdate(string talkTitle, string rating, int totalVotes)
    {
        await Clients.All.SendAsync("VoteUpdate", talkTitle, rating, totalVotes);
    }

    public async Task SendTalkUpdate(string currentTalk, string nextTalk, int remainingSeconds, int progressPercent)
    {
        await Clients.All.SendAsync("TalkUpdate", currentTalk, nextTalk, remainingSeconds, progressPercent);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
