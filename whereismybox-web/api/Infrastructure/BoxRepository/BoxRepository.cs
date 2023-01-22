using Domain.Models;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Infrastructure.BoxRepository;

public class BoxRepository : IBoxRepository
{
    private readonly Container _container;

    public BoxRepository(BoxRepositoryConfiguration config)
    {
        var cosmosClient = new CosmosClient(config.ConnectionString);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }

    public async Task<Box> Add(Guid userId, Box box)
    {
        var cosmosAware = CosmosAwareBox.ToCosmosAware(box);
        var response = await _container.CreateItemAsync(cosmosAware, new PartitionKey(cosmosAware.PartitionKey));
        return response.Resource;
    }

    public async Task<Box> Get(Guid userId, Guid boxId)
    {
        var cosmosAware =
            await _container.ReadItemAsync<CosmosAwareBox>(boxId.ToString(), new PartitionKey(userId.ToString()));
        return cosmosAware.Resource;
    }

    public async Task<Box> PersistUpdate(Guid userId, Box updatedBox)
    {
        if (updatedBox is not CosmosAwareBox cosmosAwareBox)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var cosmosResponse =
            await _container.ReplaceItemAsync(cosmosAwareBox, cosmosAwareBox.Id,
                new PartitionKey(cosmosAwareBox.PartitionKey), new ItemRequestOptions()
                {
                    IfMatchEtag = cosmosAwareBox.ETag
                });
        return cosmosResponse.Resource;
    }

    public async Task<List<Box>> ListBoxesByUser(Guid userId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareBox>(requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(userId.ToString())
            });

        var iterator = queryAble.ToFeedIterator();
        var results = new List<Box>();
        while (iterator.HasMoreResults)
        {
            results.AddRange(await iterator.ReadNextAsync());
        }
    
        return results.OrderBy(b => b.Number).ToList();
    }
}