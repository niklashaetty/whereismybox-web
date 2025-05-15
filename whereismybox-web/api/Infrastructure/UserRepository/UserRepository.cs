using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using InvalidOperationException = System.InvalidOperationException;
using User = Domain.Models.User;

namespace Infrastructure.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly Container _container;

    public UserRepository(CosmosClient cosmosClient, UserRepositoryConfiguration config)
    {
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }
    
    public async Task<User> Create(User user)
    {
        var cosmosAware = CosmosAwareUser.ToCosmosAware(user);
        var response = await _container.CreateItemAsync(cosmosAware, cosmosAware.GetPartitionKey());
        return response.Resource;
    }

    public async Task<User?> Get(UserId userId)
    {
        try
        {
            var cosmosAware =
                await _container.ReadItemAsync<CosmosAwareUser>(userId.ToString(), new PartitionKey(userId.ToString()));
            return cosmosAware.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
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