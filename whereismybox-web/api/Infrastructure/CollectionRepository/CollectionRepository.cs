using System.Net;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.BoxRepository;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Infrastructure.CollectionRepository;

public class CollectionRepository : ICollectionRepository
{
    private readonly Container _container;

    public CollectionRepository(CollectionRepositoryConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var cosmosClient = new CosmosClient(config.ConnectionString);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }

    public async Task<Collection> CreateCollection(Collection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        var cosmosAware = CosmosAwareCollection.ToCosmosAwareCollection(collection);
        var result = await _container.CreateItemAsync(collection, cosmosAware.PartitionKey());
        return result.Resource;
    }
    
    public async Task<Collection> GetCollection(CollectionId collectionId)
    {
        try
        {
            var cosmosAware =
                await _container.ReadItemAsync<CosmosAwareCollection>(collectionId.ToString(),
                    new PartitionKey(collectionId.ToString()));
            return cosmosAware.Resource;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            throw new CollectionNotFoundException(collectionId);
        }
    }

    public async Task<List<Collection>> FindCollectionsByOwner(UserId ownerId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareCollection>(requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(ownerId.ToString())
            });

        var iterator = queryAble.ToFeedIterator();
        var collections = new List<Collection>();
        while (iterator.HasMoreResults)
        {
            collections.AddRange(await iterator.ReadNextAsync());
        }

        return collections;
    }

    public async Task<Collection> PersistUpdate(Collection updatedCollection)
    {
        if (updatedCollection is not CosmosAwareCollection cosmosAwareCollection)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var cosmosResponse =
            await _container.ReplaceItemAsync(cosmosAwareCollection, cosmosAwareCollection.Id,
                cosmosAwareCollection.PartitionKey(), new ItemRequestOptions()
                {
                    IfMatchEtag = cosmosAwareCollection.ETag
                });
        return cosmosResponse.Resource;
    }
}