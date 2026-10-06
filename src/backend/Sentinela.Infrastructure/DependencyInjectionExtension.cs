using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Sentinela.Domain.Identity;
using Sentinela.Domain.Messaging;
using Sentinela.Domain.Repositories;
using Sentinela.Domain.Repositories.Asset;
using Sentinela.Domain.Repositories.Sample;
using Sentinela.Domain.Repositories.User;
using Sentinela.Domain.Security.ApiKeyHashing;
using Sentinela.Domain.Security.PasswordHashing;
using Sentinela.Domain.Security.Tokens;
using Sentinela.Infrastructure.DataAccess;
using Sentinela.Infrastructure.DataAccess.Repositories;
using Sentinela.Infrastructure.Identity;
using Sentinela.Infrastructure.Messaging;
using Sentinela.Infrastructure.Security.ApiKeyHashing;
using Sentinela.Infrastructure.Security.PasswordHashing;
using Sentinela.Infrastructure.Security.Tokens.Access;

namespace Sentinela.Infrastructure;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddMessaging(configuration);
            services.AddTokens(configuration);
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddDbContext(configuration);
            services.AddScoped<ILoggedUser, LoggedUser>();
            services.AddScoped<IApiKeyHasher, Sha256ApiKeyHasher>();
            
        }

        public void AddWorkerInfrastructure(IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddDbContext(configuration);
            services.AddMessaging(configuration);
        }

        private void AddRepositories()
        {
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IAssetReadOnlyRepository, AssetRepository>();
            services.AddScoped<IAssetWriteOnlyRepository, AssetRepository>();
            services.AddScoped<IAssetUpdateOnlyRepository, AssetRepository>();
            services.AddScoped<ISampleWriteOnlyRepository, SampleRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private void AddTokens(IConfiguration configuration) 
        {
            services.AddScoped<IAccessTokenGenerator>(provider =>
            {
                var expirationTime = configuration.GetValue<uint>("Jwt:ExpirationTime");
                var signingKey = configuration.GetValue<string>("Jwt:SigningKey");

                return new JwtTokenHandler(expirationTime, signingKey!);
            });
        }

        private void AddDbContext(IConfiguration configuration)
        {
            services.AddDbContext<SentinelaDbContext>(config =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection");

                config.UseMySQL(connectionString!);
            });
        }

        private void AddMessaging(IConfiguration configuration)
        {
            services.AddSingleton<IConnection>(_ =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration.GetValue<string>("RabbitMq:HostName")!,
                    UserName = configuration.GetValue<string>("RabbitMq:UserName")!,
                    Password = configuration.GetValue<string>("RabbitMq:Password")!
                };

                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

            services.AddScoped<IQueuePublisher, RabbitMqPublisher>();
        }
    }
}