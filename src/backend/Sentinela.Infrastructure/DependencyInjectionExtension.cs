using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentinela.Domain.Repositories;
using Sentinela.Domain.Repositories.User;
using Sentinela.Domain.Security.PasswordHashing;
using Sentinela.Infrastructure.DataAccess;
using Sentinela.Infrastructure.DataAccess.Repositories;
using Sentinela.Infrastructure.Security.PasswordHashing;

namespace Sentinela.Infrastructure;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<SentinelaDbContext>(config =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection");

                config.UseMySQL(connectionString!);
            });
        }
    }
}