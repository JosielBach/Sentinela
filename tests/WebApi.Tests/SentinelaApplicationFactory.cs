using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentinela.Domain.Security.PasswordHashing;
using Sentinela.Infrastructure.DataAccess;
using Testcontainers.MySql;
using WebApi.Tests.Resources;

namespace WebApi.Tests;

public class SentinelaApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public UserIdentityManager User01 { get; private set; }
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

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var (user, password) = UserBuilder.Build();

        user.Password = passwordHasher.HashPassword(password);

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User01 = new UserIdentityManager(user, password);
    }
    Task IAsyncLifetime.DisposeAsync() => _mySqlContainer.StopAsync();
  
}
