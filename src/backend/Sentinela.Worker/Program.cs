using Sentinela.Application;
using Sentinela.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWorkerApplication();
builder.Services.AddWorkerInfrastructure(builder.Configuration);

var host = builder.Build();
host.Run();
