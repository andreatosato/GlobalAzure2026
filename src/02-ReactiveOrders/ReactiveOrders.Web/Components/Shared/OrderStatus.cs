using MudBlazor;

namespace ReactiveOrders.Web.Components.Shared;

public static class OrderStatus
{
    public const string Placed = "Placed";
    public const string Confirmed = "Confirmed";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";

    public static Color ToColor(string status) => status switch
    {
        Placed => Color.Warning,
        Confirmed => Color.Success,
        Rejected => Color.Error,
        Cancelled => Color.Default,
        _ => Color.Info
    };

    public static string ToIcon(string status) => status switch
    {
        Placed => Icons.Material.Filled.Schedule,
        Confirmed => Icons.Material.Filled.CheckCircle,
        Rejected => Icons.Material.Filled.Cancel,
        Cancelled => Icons.Material.Filled.Block,
        _ => Icons.Material.Filled.Info
    };

    public static string ToLabel(string status) => status switch
    {
        Placed => "In attesa",
        Confirmed => "Confermato",
        Rejected => "Rifiutato",
        Cancelled => "Annullato",
        _ => status
    };
}
