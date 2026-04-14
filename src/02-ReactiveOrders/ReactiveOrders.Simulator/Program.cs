using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveOrders.Simulator;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddHttpClient("commandapi", client =>
{
    client.BaseAddress = new Uri("https+http://commandapi");
});
builder.Services.AddHostedService<OrderSimulator>();
builder.Build().Run();
