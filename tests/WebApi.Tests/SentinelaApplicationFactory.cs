using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MySql;

namespace WebApi.Tests;

public class SentinelaApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer;
    public SentinelaApplicationFactory()
    {
        var schema = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "schema.sql"));

        _mySqlContainer = new MySqlBuilder("mysql:8.0")
            .WithResourceMapping(schema, "/docker-entrypoint-initdb.d/schema.sql")
            .Build();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Tests").ConfigureAppConfiguration((_, configuration) => 
        {
            var parameters = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DbConnection"] = _mySqlContainer.GetConnectionString()
            };

            configuration.AddInMemoryCollection(parameters);
        });
    }
    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }
    Task IAsyncLifetime.DisposeAsync() => _mySqlContainer.StopAsync();
  
}
