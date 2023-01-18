using Domain.Repositories;
using Domain.Services.BoxCreationService;
using Domain.Services.ItemAddingService;
using Domain.Services.ItemDeletionService;
using Domain.Services.UserCreationService;
using Functions;
using Infrastructure;
using Infrastructure.BoxRepository;
using Infrastructure.UserRepository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            // Repositories
            builder.Services.AddSingleton<IBoxRepository, InMemoryBoxRepository>();
            builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            
            // Services
            builder.Services.AddSingleton<IUserCreationService, UserCreationService>();
            builder.Services.AddSingleton<IBoxCreationService, BoxCreationService>();
            builder.Services.AddSingleton<IItemAddingService, ItemAddingService>();
            builder.Services.AddSingleton<IItemDeletionService, ItemDeletionService>();
            
            
            builder.Services.AddMvcCore().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }
    }
}