var builder = DistributedApplication.CreateBuilder(args);

var signalr = builder.AddAzureSignalR("signalr")
    .RunAsEmulator();

var web = builder.AddProject<Projects.ReactiveBoard_Web>("web")
    .WithReference(signalr)
    .WaitFor(signalr)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.ReactiveBoard_Simulator>("simulator")
    .WithReference(signalr)
    .WaitFor(signalr)
    .WithReference(web)
    .WaitFor(web);

builder.Build().Run();
