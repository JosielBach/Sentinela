using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Sentinela.Application;
using Sentinela.Infrastructure;
using Sentinela.Infrastructure.DataAccess;
using Sentinela.Worker.Workers;
using Testcontainers.MySql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Worker.Tests;

public class SentinelaWorkerFactory : IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;
    private readonly RabbitMqContainer _rabbitMqContainer;
    private readonly RedisContainer _redisContainer;
    private IHost _host = default!;
    public IConnection Connection { get; private set; } = default!;
    public IServiceProvider Services => _host.Services;
    public SentinelaWorkerFactory()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0").WithDatabase("sentinelaDb")
            .WithCommand("--innodb-use-native-aio=0").Build();
        _rabbitMqContainer = new RabbitMqBuilder("rabbitmq:4-management").Build();
        _redisContainer = new RedisBuilder("redis:7-alpine").Build();
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_mySqlContainer.StartAsync(), _rabbitMqContainer.StartAsync(), _redisContainer.StartAsync());

        var connectionFactory = new ConnectionFactory { Uri = new Uri(_rabbitMqContainer.GetConnectionString()) };
        Connection = await connectionFactory.CreateConnectionAsync();

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { EnvironmentName = "Tests" });

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DbConnection"] = _mySqlContainer.GetConnectionString(),
            ["Redis:ConnectionString"] = _redisContainer.GetConnectionString()
        });

        builder.Services.AddWorkerApplication();
        builder.Services.AddWorkerInfrastructure(builder.Configuration);
        builder.Services.RemoveAll<IConnection>();
        builder.Services.AddSingleton(Connection);
        builder.Services.AddHostedService<IngestionConsumerWorker>();

        _host = builder.Build();

        await using (var scope = _host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        await _host.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
        await Connection.DisposeAsync();
        await Task.WhenAll(_mySqlContainer.StopAsync(), _rabbitMqContainer.StopAsync(), _redisContainer.StopAsync());
    }
}
