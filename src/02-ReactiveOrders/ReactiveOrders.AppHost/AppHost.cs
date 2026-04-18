var builder = DistributedApplication.CreateBuilder(args);

// In publish mode aggiungiamo un Azure Container App Environment come compute host.
// Serve anche per abilitare le role assignment (managed identity) su SignalR e Service Bus.
if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddAzureContainerAppEnvironment("cae");
}

// Azure SignalR viene aggiunto solo in fase di publish (deploy in produzione).
// In locale (run) si usa il SignalR integrato in ASP.NET Core, quindi nessuna risorsa.
var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR("signalr")
    : null;

var messaging = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator();

var topic = messaging.AddServiceBusTopic("order-events");
topic.AddServiceBusSubscription("event-processor");

var sqlServer = builder.AddSqlServer("sql");
var ordersDb = sqlServer.AddDatabase("ordersdb");
var readDb = sqlServer.AddDatabase("readdb");

var commandApi = builder.AddProject<Projects.ReactiveOrders_CommandApi>("commandapi")
    .WithReference(ordersDb)
    .WaitFor(ordersDb)
    .WithReference(messaging)
    .WaitFor(messaging);

var readApi = builder.AddProject<Projects.ReactiveOrders_ReadApi>("readapi")
    .WithReference(readDb)
    .WaitFor(readDb);

var web = builder.AddProject<Projects.ReactiveOrders_Web>("web")
    .WithReference(commandApi)
    .WithReference(readApi)
    .WithExternalHttpEndpoints();

if (signalr is not null)
{
    web = web.WithReference(signalr).WaitFor(signalr);
}

var eventProcessor = builder.AddProject<Projects.ReactiveOrders_EventProcessor>("eventprocessor")
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(readDb)
    .WaitFor(readDb)
    .WithReference(web)
    .WaitFor(web);

if (signalr is not null)
{
    eventProcessor = eventProcessor.WithReference(signalr).WaitFor(signalr);
}

builder.AddProject<Projects.ReactiveOrders_Simulator>("simulator")
    .WithReference(commandApi)
    .WaitFor(commandApi);

builder.Build().Run();
