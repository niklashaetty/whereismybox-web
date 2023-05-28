using System.IO;
using System.Net.Http.Formatting;
using Domain.Repositories;
using Domain.Services.BoxCreationService;
using Domain.Services.ItemAddingService;
using Domain.Services.ItemDeletionService;
using Domain.Services.ItemEditingService;
using Domain.Services.UnattachedItemFetchingService;
using Domain.Services.UserCreationService;
using Functions;
using Infrastructure.BoxRepository;
using Infrastructure.UnattachedItemRepository;
using Infrastructure.UserRepository;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Logging
            builder.Services.AddLogging();

            // Config
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            
            // Cosmos
            builder.Services.AddSingleton(new BoxRepositoryConfiguration(
                config["CosmosConnectionString"], 
                "WhereIsMyBox", "Boxes"));
            builder.Services.AddSingleton(new UserRepositoryConfiguration(
                config["CosmosConnectionString"], 
                "WhereIsMyBox", "Users"));
            builder.Services.AddSingleton(new UnattachedItemRepositoryRepositoryConfiguration(
                config["CosmosConnectionString"], 
                "WhereIsMyBox", "UnattachedItems"));

            builder.Services.AddLogging();
            
            // Repositories
            builder.Services.AddSingleton<IBoxRepository, BoxRepository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            
            // Services
            builder.Services.AddSingleton<IUserCreationService, UserCreationService>();
            builder.Services.AddSingleton<IBoxCreationService, BoxCreationService>();
            builder.Services.AddSingleton<IItemAddingService, ItemAddingService>();
            builder.Services.AddSingleton<IItemDeletionService, ItemDeletionService>();
            builder.Services.AddSingleton<IItemEditingService, ItemEditingService>();
            builder.Services.AddSingleton<IUnattachedItemRepository, UnattachedItemRepository>();
            builder.Services.AddSingleton<IUnattachedItemFetchingService, UnattachedItemFetchingService>();

            builder.Services.AddMvcCore().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
        }
    }
}