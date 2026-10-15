using Sentinela.Application;
using Sentinela.Infrastructure;
using Sentinela.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWorkerApplication();
builder.Services.AddWorkerInfrastructure(builder.Configuration);
builder.Services.AddHostedService<IngestionConsumerWorker>();

var host = builder.Build();
host.Run();
