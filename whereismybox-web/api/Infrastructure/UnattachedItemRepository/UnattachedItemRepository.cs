using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Infrastructure.UnattachedItemRepository;

public class UnattachedItemRepository : IUnattachedItemRepository
{
    private readonly Container _container;

    public UnattachedItemRepository(UnattachedItemRepositoryConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var cosmosClient = new CosmosClient(config.ConnectionString);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }
    
    public async Task<UnattachedItem> Create(UnattachedItem unattachedItem)
    {
        var cosmosAware = CosmosAwareUnattachedItem.ToCosmosAware(unattachedItem);
        var response = await _container.CreateItemAsync(cosmosAware, cosmosAware.PartitionKey());
        return response.Resource;
    }

    public async Task<List<UnattachedItem>> GetCollection(CollectionId collectionId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareUnattachedItem>(requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(collectionId.ToString())
            });

        var iterator = queryAble.ToFeedIterator();
        var results = new List<UnattachedItem>();
        while (iterator.HasMoreResults)
        {
            results.AddRange(await iterator.ReadNextAsync());
        }

        return results;
    }

    public async Task Delete(CollectionId collectionId, ItemId itemId)
    {
        await _container.DeleteItemAsync<CosmosAwareUnattachedItem>(itemId.ToString(),
            new PartitionKey(collectionId.ToString()));
    }
}