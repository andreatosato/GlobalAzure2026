namespace ReactiveBoard.Web.Models;

public record ConferenceUpdate(
    int AttendeeCount,
    string CurrentTalk,
    string NextTalk,
    TimeSpan RemainingTime,
    Dictionary<string, int> LiveVotes
);

public record VoteUpdate(string TalkTitle, string Rating, int TotalVotes);

public record AttendeeUpdate(int Count, string Action);
