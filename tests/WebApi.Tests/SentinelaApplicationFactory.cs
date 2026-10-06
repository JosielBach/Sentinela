using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sentinela.Domain.Messaging;
using Sentinela.Domain.Security.ApiKeyHashing;
using Sentinela.Domain.Security.PasswordHashing;
using Sentinela.Domain.Security.Tokens;
using Sentinela.Infrastructure.DataAccess;
using Testcontainers.MySql;
using WebApi.Tests.Resources;

namespace WebApi.Tests;

public class SentinelaApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public UserIdentityManager User01 { get; private set; } = default!;
    public string TOKEN_USER_NOT_FOUND_IN_DATA_BASE { get; private set; } = string.Empty;
    public FakeQueuePublisher QueuePublisher { get; } = new();
    private readonly MySqlContainer _mySqlContainer;
    public string ACTIVE_ASSET_API_KEY { get; } = "active-asset-api-key";
    public string INACTIVE_ASSET_API_KEY { get; } = "inactive-asset-api-key";
    public Guid ActiveAssetId { get; private set; }
    public SentinelaApplicationFactory()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0").WithDatabase("sentinelaDb")
            .WithCommand("--innodb-use-native-aio=0").Build();
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
        }).ConfigureTestServices(services =>
        {
            services.RemoveAll<IQueuePublisher>();
            services.AddSingleton<IQueuePublisher>(QueuePublisher);
        });
    }
    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var accessTokenGenerator = scope.ServiceProvider.GetRequiredService<IAccessTokenGenerator>();
        var apiKeyHasher = scope.ServiceProvider.GetRequiredService<IApiKeyHasher>();

        await dbContext.Database.MigrateAsync();

        var (user, password) = UserBuilder.Build();
        user.Password = passwordHasher.HashPassword(password);

        var assets = AssetBuilder.Collection(2);
        assets[0].ApiKeyHash = apiKeyHasher.HashApiKey(ACTIVE_ASSET_API_KEY);
        assets[1].ApiKeyHash = apiKeyHasher.HashApiKey(INACTIVE_ASSET_API_KEY);
        assets[1].Active = false;

        await dbContext.Assets.AddRangeAsync(assets);
        ActiveAssetId = assets[0].Id;

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        var user1AccessToken = accessTokenGenerator.Generate(user);

        User01 = new UserIdentityManager(user, password, user1AccessToken);
        TOKEN_USER_NOT_FOUND_IN_DATA_BASE = accessTokenGenerator.Generate(new Sentinela.Domain.Entities.User());
    }
    Task IAsyncLifetime.DisposeAsync() => _mySqlContainer.StopAsync();
  
}
