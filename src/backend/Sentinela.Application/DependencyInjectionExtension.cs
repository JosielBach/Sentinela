using Microsoft.Extensions.DependencyInjection;
using Sentinela.Application.UseCases.Login.WithEmailAndPassword;
using Sentinela.Application.UseCases.User.Register;

namespace Sentinela.Application;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services) 
    { 
        public void AddApplication() 
        { 
            services.AddScoped<IRegisterUserAccountUseCase, RegisterUserAccountUseCase>();
            services.AddScoped<ILoginWithEmailAndPasswordUseCase, LoginWithEmailAndPasswordUseCase>();
        }
    }
}
