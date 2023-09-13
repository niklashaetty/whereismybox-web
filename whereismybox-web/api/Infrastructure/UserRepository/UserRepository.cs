using Domain.Exceptions;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
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

    public async Task<User?> SearchByUsername(string username)
    {
        var query = new QueryDefinition(
            $"SELECT * FROM c WHERE c.{nameof(CosmosAwareUser.Username)} = '{username}'");
        var iterator = _container.GetItemQueryIterator<CosmosAwareUser>(query);

        if (!iterator.HasMoreResults)
        {
            return null;
        }

        var response = await iterator.ReadNextAsync();
        
        return response.Resource.FirstOrDefault();
    }

    public async Task<User?> SearchByExternalUserId(ExternalUserId externalUserId)
    {
        var query = new QueryDefinition(
            $"SELECT * FROM c WHERE c.{nameof(CosmosAwareUser.ExternalUserId)} = '{externalUserId}'");
        var iterator = _container.GetItemQueryIterator<CosmosAwareUser>(query);

        if (!iterator.HasMoreResults)
        {
            return null;
        }

        var response = await iterator.ReadNextAsync();
        
        return response.Resource.FirstOrDefault();
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

    public async Task<List<User>> SearchByCollectionId(CollectionId collectionId)
    {
        var query = new QueryDefinition(
            $@"SELECT * FROM c WHERE ARRAY_CONTAINS(c.ContributorCollections, '{collectionId.Value}')");
        var iterator = _container.GetItemQueryIterator<CosmosAwareUser>(query);

        if (!iterator.HasMoreResults)
        {
            return new List<User>();
        }

        var response = await iterator.ReadNextAsync();
        var res = new List<User>();
        res.AddRange(response.Resource.ToList());
        return res;
    }

    public async Task<User> GetCollectionOwner(CollectionId collectionId)
    {
        var query = new QueryDefinition(

            $"SELECT * FROM c WHERE c.{nameof(CosmosAwareUser.PrimaryCollectionId)} = '{collectionId}'");
        var iterator = _container.GetItemQueryIterator<CosmosAwareUser>(query);

        if (!iterator.HasMoreResults)
        {
            throw new CollectionNotFoundException(collectionId);
        }

        var response = await iterator.ReadNextAsync();

        return response.Resource.First();
    }
    
    private async Task<List<User>> GetCollectionContributors(CollectionId collectionId)
    {
        var query = new QueryDefinition(
            $@"SELECT * FROM c WHERE ARRAY_CONTAINS(c.{nameof(CosmosAwareUser.ContributorCollections)}, {collectionId})"
        );
        var iterator = _container.GetItemQueryIterator<CosmosAwareUser>(query);

        if (!iterator.HasMoreResults)
        {
            return new List<User>();
        }

        var response = await iterator.ReadNextAsync();
        var res = new List<User>();
        res.AddRange(response.Resource.ToList());
        return res;
    }
}