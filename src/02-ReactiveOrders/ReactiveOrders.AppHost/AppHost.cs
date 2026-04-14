var builder = DistributedApplication.CreateBuilder(args);

var signalr = builder.AddAzureSignalR("signalr")
    .RunAsEmulator();

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
    .WithReference(signalr)
    .WaitFor(signalr)
    .WithReference(commandApi)
    .WithReference(readApi)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.ReactiveOrders_EventProcessor>("eventprocessor")
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(readDb)
    .WaitFor(readDb)
    .WithReference(signalr)
    .WaitFor(signalr)
    .WithReference(web)
    .WaitFor(web);

builder.AddProject<Projects.ReactiveOrders_Simulator>("simulator")
    .WithReference(commandApi)
    .WaitFor(commandApi);

builder.Build().Run();
