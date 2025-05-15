using System.Net;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using User = Microsoft.Azure.Cosmos.User;

namespace Infrastructure.CollectionRepository;

public class CollectionRepository : ICollectionRepository
{
    private readonly Container _container;

    public CollectionRepository(CosmosClient cosmosClient, CollectionRepositoryConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }

    public async Task<Collection> Create(Collection collection)
    {
        var cosmosAware = CosmosAwareCollection.ToCosmosAware(collection);
        var response = await _container.CreateItemAsync(cosmosAware, PartitionKey.None);
        return response.Resource;
    }

    public async Task Delete(CollectionId collectionId)
    {
        await _container.DeleteItemAsync<CosmosAwareCollection>(collectionId.ToString(), PartitionKey.None);
    }

    public async Task<Collection> Get(CollectionId collectionId)
    {
        try
        {
            var cosmosAware =
                await _container.ReadItemAsync<CosmosAwareCollection>(collectionId.ToString(), PartitionKey.None);
            return cosmosAware.Resource;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            throw new CollectionNotFoundException(collectionId);
        }
    }


    public async Task<Collection> PersistUpdate(Collection updatedCollection)
    {
        if (updatedCollection is not CosmosAwareCollection cosmosAwareCollection)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        try
        {
            var cosmosResponse = await _container.ReplaceItemAsync(cosmosAwareCollection, cosmosAwareCollection.Id,
                PartitionKey.None, new ItemRequestOptions()
                {
                    IfMatchEtag = cosmosAwareCollection.ETag
                });
            return cosmosResponse.Resource;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            throw new CollectionNotFoundException(updatedCollection.CollectionId);
        }
    }

    public async Task<List<Collection>> GetOwnedCollections(UserId userId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareCollection>()
            .Where(c => c.Owner == userId);

        var iterator = queryAble.ToFeedIterator();
        var results = new List<Collection>();
        while (iterator.HasMoreResults)
        {
            results.AddRange(await iterator.ReadNextAsync());
        }

        return results;
    }

    public async Task<List<Collection>> GetCollectionsWhereUserIsContributor(UserId userId)
    {
        var query = new QueryDefinition(
            $@"SELECT * FROM c WHERE ARRAY_CONTAINS(c.Contributors, '{userId.Value}')");
        var iterator = _container.GetItemQueryIterator<Collection>(query);

        if (!iterator.HasMoreResults)
        {
            return new List<Collection>();
        }

        var response = await iterator.ReadNextAsync();
        var res = new List<Collection>();
        res.AddRange(response.Resource.ToList());
        return res;
    }
}