var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WFNSystem_API>("apiservice")
    .WithHttpHealthCheck("/health");

builder.Build().Run();