using ReactiveOrders.Web.Components;
using ReactiveOrders.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR()
    .AddNamedAzureSignalR("signalr");

builder.Services.AddHttpClient("CommandApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["services:commandapi:http:0"] ?? "http://localhost:5001");
});

builder.Services.AddHttpClient("ReadApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["services:readapi:http:0"] ?? "http://localhost:5002");
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<OrderHub>("/orderhub");

app.Run();
