using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using InvalidOperationException = System.InvalidOperationException;
using User = Domain.Models.User;

namespace Infrastructure.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly Container _container;

    public UserRepository(UserRepositoryConfiguration config)
    {
        var cosmosClient = new CosmosClient(config.ConnectionString);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }
    
    public async Task<User> Create(User user)
    {
        var cosmosAware = CosmosAwareUser.ToCosmosAware(user);
        var response = await _container.CreateItemAsync(cosmosAware, cosmosAware.GetPartitionKey());
        return response.Resource;
    }

    public async Task<User> Get(Guid userId)
    {
        var cosmosAware =
            await _container.ReadItemAsync<CosmosAwareUser>(userId.ToString(), new PartitionKey(userId.ToString()));
        return cosmosAware.Resource;
    }

    public async Task<User> PersistUpdate(User updatedUser)
    {
        if (updatedUser is not CosmosAwareUser cosmosAwareUser)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var cosmosResponse =
            await _container.ReplaceItemAsync(cosmosAwareUser, cosmosAwareUser.Id,
                cosmosAwareUser.GetPartitionKey(), new ItemRequestOptions()
                {
                    IfMatchEtag = cosmosAwareUser.ETag
                });
        return cosmosResponse.Resource;
    }
}