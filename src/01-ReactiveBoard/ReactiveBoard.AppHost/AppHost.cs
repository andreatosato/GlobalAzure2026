var builder = DistributedApplication.CreateBuilder(args);

var web = builder.AddProject<Projects.ReactiveBoard_Web>("web")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.ReactiveBoard_Simulator>("simulator")
    .WithReference(web)
    .WaitFor(web);

builder.Build().Run();
