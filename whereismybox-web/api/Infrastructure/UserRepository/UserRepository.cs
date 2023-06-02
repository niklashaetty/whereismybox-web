using Domain.Exceptions;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
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

    public async Task<User> Get(UserId userId)
    {
        var cosmosAware =
            await _container.ReadItemAsync<CosmosAwareUser>(userId.ToString(), new PartitionKey(userId.ToString()));
        return cosmosAware.Resource;
    }

    public async Task<User> Get(CollectionId collectionId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareUser>()
            .Where(u => u.PrimaryCollectionId == collectionId);

        var iterator = queryAble.ToFeedIterator();
        var results = new List<CosmosAwareUser>();
        while (iterator.HasMoreResults)
        {
            results.AddRange(await iterator.ReadNextAsync());
        }

        var res = results.FirstOrDefault();
        if (res is null)
        {
            throw new UserNotFoundException(collectionId);
        }

        return res;
    }
    
    public async Task<User> Get(ExternalUserId externalUserId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareUser>()
            .Where(u => u.ExternalUserId == externalUserId);

        var iterator = queryAble.ToFeedIterator();
        var results = new List<CosmosAwareUser>();
        while (iterator.HasMoreResults)
        {
            results.AddRange(await iterator.ReadNextAsync());
        }

        var res = results.FirstOrDefault();
        if (res is null)
        {
            throw new UserNotFoundException(externalUserId);
        }

        return res;
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