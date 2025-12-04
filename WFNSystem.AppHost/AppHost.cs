var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.WFNSystem_API>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddViteApp("frontend", "../WFN.UI")
    .WithReference(apiservice);

builder.Build().Run();