using System.Collections.Generic;
using System.IO;
using Domain.Authorization;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Models;
using Domain.Queries;
using Domain.QueryHandlers;
using Domain.Repositories;
using Infrastructure.BoxRepository;
using Infrastructure.CollectionRepository;
using Infrastructure.UnattachedItemRepository;
using Infrastructure.UserRepository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using User = Domain.Models.User;


// Config
var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", true, true)
    .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        // Logging
        services.AddLogging();
        
        // Cosmos
        services.AddSingleton(s => new CosmosClient(config["CosmosConnectionString"]));
        services.AddSingleton(new BoxRepositoryConfiguration("WhereIsMyBox", "BoxesV2"));
        services.AddSingleton(new UserRepositoryConfiguration(
            "WhereIsMyBox", "UsersV2"));
        services.AddSingleton(new UnattachedItemRepositoryConfiguration(
            "WhereIsMyBox", "UnattachedItemsV2"));
        services.AddSingleton(new CollectionRepositoryConfiguration(
            "WhereIsMyBox", "Collections"));
        
        // Authorization
        services.AddSingleton<IAuthorizationService, AuthorizationService>();
        
        // CommandHandlers
        services.AddSingleton<ICommandHandler<AddItemCommand>, AddItemCommandHandler>();
        services.AddSingleton<ICommandHandler<MoveItemCommand>, MoveItemCommandHandler>();
        services.AddSingleton<ICommandHandler<CreateCollectionCommand>, CreateCollectionCommandHandler>();
        services.AddSingleton<ICommandHandler<CreateBoxCommand>, CreateBoxCommandHandler>();
        services.AddSingleton<ICommandHandler<CreateUserCommand>,CreateUserCommandHandler >();
        services.AddSingleton<ICommandHandler<DeleteBoxCommand>, DeleteBoxCommandHandler>();
        services.AddSingleton<ICommandHandler<UpdateBoxCommand>, UpdateBoxCommandHandler>();
        services.AddSingleton<ICommandHandler<DeleteItemCommand>, DeleteItemCommandHandler>();
        services.AddSingleton<ICommandHandler<DeleteCollectionCommand>, DeleteCollectionCommandHandler>();
        services.AddSingleton<ICommandHandler<DeleteUnattachedItemCommand>, DeleteUnattachedItemCommandHandler>();
        services.AddSingleton<ICommandHandler<MoveUnattachedItemToBoxCommand>, MoveUnattachedItemToCommandHandler>();
        services.AddSingleton<ICommandHandler<AddContributorCommand>, AddContributorCommandHandler>();
        services.AddSingleton<ICommandHandler<DeleteContributorCommand>, DeleteContributorCommandHandler>();
        services.AddSingleton<ICommandHandler<RegisterUserCommand>, RegisterUserCommandHandler>();

        // QueryHandlers
        services.AddSingleton<IQueryHandler<GetBoxCollectionQuery, List<Box>>, GetBoxCollectionQueryHandler>();
        services.AddSingleton<IQueryHandler<GetUserPermissionsQuery, Permissions>, GetUserPermissionsQueryHandler>();
        services.AddSingleton<IQueryHandler<GetBoxQuery, Box>, GetBoxQueryHandler>();
        services.AddSingleton<IQueryHandler<GetUserByExternalUserIdQuery, User>, GetUserByExternalUserIdQueryHandler>();
        services.AddSingleton<IQueryHandler<GetUnattachedItemsQuery, List<UnattachedItem>>, GetUnattachedItemsQueryHandler>();
        services.AddSingleton<IQueryHandler<GetCollectionContributorsQuery, List<User>>, GetCollectionContributorsQueryHandler>();
        services.AddSingleton<IQueryHandler<GetCollectionOwnerQuery, User>, GetCollectionOwnerQueryHandler>();
        services.AddSingleton<IQueryHandler<GetCollectionQuery, Collection>, GetCollectionQueryHandler>();
        services.AddSingleton<IQueryHandler<GetOwnedCollectionQuery, List<Collection>>, GetOwnedCollectionsQueryHandler>();
        services.AddSingleton<IQueryHandler<GetContributorCollectionsQuery, List<Collection>>, GetContributorCollectionsQueryHandler>();
            
        // Repositories
        services.AddSingleton<IBoxRepository, BoxRepository>();
        services.AddSingleton<ICollectionRepository, CollectionRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUnattachedItemRepository, UnattachedItemRepository>();

        services.AddMvcCore().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
            options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        });
        
    })
    .Build();

host.Run();