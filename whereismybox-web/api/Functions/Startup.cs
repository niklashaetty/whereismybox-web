using System.Collections.Generic;
using System.IO;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Queries;
using Domain.QueryHandlers;
using Domain.Repositories;
using Functions;
using Infrastructure.BoxRepository;
using Infrastructure.CollectionRepository;
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
                "WhereIsMyBox", "BoxesV2"));
            builder.Services.AddSingleton(new UserRepositoryConfiguration(
                config["CosmosConnectionString"], 
                "WhereIsMyBox", "UsersV2"));
            builder.Services.AddSingleton(new UnattachedItemRepositoryConfiguration(
                config["CosmosConnectionString"], 
                "WhereIsMyBox", "UnattachedItemsV2"));
            builder.Services.AddSingleton(new CollectionRepositoryConfiguration(
                config["CosmosConnectionString"], 
                "WhereIsMyBox", "Collections"));

            builder.Services.AddLogging();
            
            // Authorization
            builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
            
            // CommandHandlers
            builder.Services.AddSingleton<ICommandHandler<AddItemCommand>, AddItemCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<MoveItemCommand>, MoveItemCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<CreateCollectionCommand>, CreateCollectionCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<CreateBoxCommand>, CreateBoxCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<CreateUserCommand>,CreateUserCommandHandler >();
            builder.Services.AddSingleton<ICommandHandler<DeleteBoxCommand>, DeleteBoxCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<DeleteItemCommand>, DeleteItemCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<DeleteCollectionCommand>, DeleteCollectionCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<DeleteUnattachedItemCommand>, DeleteUnattachedItemCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<MoveUnattachedItemToBoxCommand>, MoveUnattachedItemToCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<AddContributorCommand>, AddContributorCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<DeleteContributorCommand>, DeleteContributorCommandHandler>();
            builder.Services.AddSingleton<ICommandHandler<RegisterUserCommand>, RegisterUserCommandHandler>();
            
            // QueryHandlers
            builder.Services.AddSingleton<IQueryHandler<GetBoxCollectionQuery, List<Box>>, GetBoxCollectionQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetUserPermissionsQuery, Permissions>, GetUserPermissionsQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetBoxQuery, Box>, GetBoxQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetUserByExternalUserIdQuery, User>, GetUserByExternalUserIdQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>>, GetUnattachedItemsQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetCollectionContributorsQuery, List<User>>, GetCollectionContributorsQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetCollectionOwnerQuery, User>, GetCollectionOwnerQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetCollectionQuery, Collection>, GetCollectionQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetOwnedCollectionQuery, List<Collection>>, GetOwnedCollectionsQueryHandler>();
            builder.Services.AddSingleton<IQueryHandler<GetContributorCollectionsQuery, List<Collection>>, GetContributorCollectionsQueryHandler>();
            
            // Repositories
            builder.Services.AddSingleton<IBoxRepository, BoxRepository>();
            builder.Services.AddSingleton<ICollectionRepository, CollectionRepository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IUnattachedItemRepository, UnattachedItemRepository>();

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