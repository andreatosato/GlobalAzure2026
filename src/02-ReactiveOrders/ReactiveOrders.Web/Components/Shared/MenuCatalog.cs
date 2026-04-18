namespace ReactiveOrders.Web.Components.Shared;

public record MenuItemInfo(string Name, decimal Price, string Emoji, string Category);

public static class MenuCatalog
{
    public static readonly IReadOnlyList<MenuItemInfo> Items =
    [
        new("Pizza Margherita",   8.50m, "🍕", "Food"),
        new("Pepperoni Pizza",   10.00m, "🍕", "Food"),
        new("Panino Gourmet",     6.00m, "🥪", "Food"),
        new("Insalata Caesar",    7.00m, "🥗", "Food"),
        new("Carbonara Pasta",    9.50m, "🍝", "Food"),
        new("Risotto ai Funghi", 11.00m, "🍚", "Food"),
        new("Bruschetta",         5.50m, "🥖", "Food"),
        new("Tiramisù",           5.00m, "🍰", "Dessert"),
        new("Tiramisu",           6.00m, "🍰", "Dessert"),
        new("Gelato",             4.50m, "🍨", "Dessert"),
        new("Acqua 0.5L",         1.50m, "💧", "Drink"),
        new("Acqua Minerale",     1.50m, "💧", "Drink"),
        new("Caffè Espresso",     1.20m, "☕", "Drink"),
        new("Espresso",           2.50m, "☕", "Drink"),
    ];

    public static string EmojiFor(string name) =>
        Items.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase))?.Emoji ?? "🍽️";
}
