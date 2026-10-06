using Microsoft.Extensions.DependencyInjection;
using Sentinela.Application.UseCases.Asset.ListAssets;
using Sentinela.Application.UseCases.Asset.Register;
using Sentinela.Application.UseCases.Login.WithEmailAndPassword;
using Sentinela.Application.UseCases.Sample.Ingest;
using Sentinela.Application.UseCases.Sample.Process;
using Sentinela.Application.UseCases.User.ChangePassword;
using Sentinela.Application.UseCases.User.Profile;
using Sentinela.Application.UseCases.User.Register;
using Sentinela.Application.UseCases.User.Update;

namespace Sentinela.Application;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services) 
    { 
        public void AddApplication() 
        { 
            services.AddScoped<IRegisterUserAccountUseCase, RegisterUserAccountUseCase>();
            services.AddScoped<ILoginWithEmailAndPasswordUseCase, LoginWithEmailAndPasswordUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
            services.AddScoped<IRegisterAssetUseCase, RegisterAssetUseCase>();
            services.AddScoped<IListAssetsUseCase, ListAssetsUseCase>();
            services.AddScoped<IIngestSampleBatchUseCase, IngestSampleBatchUseCase>();
        }
        public void AddWorkerApplication()
        { 
            services.AddScoped<IProcessSampleBatchUseCase, ProcessSampleBatchUseCase>();
        }
    }
}
