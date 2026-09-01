using Microsoft.Extensions.DependencyInjection;
using Sentinela.Application.UseCases.User.Register;

namespace Sentinela.Application;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services) 
    { 
        public void AddApplication() 
        { 
            services.AddScoped<IRegisterUserAccountUseCase, RegisterUserAccountUseCase>();
        }
    }
}
