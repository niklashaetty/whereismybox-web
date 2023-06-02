using System.Net;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Infrastructure.BoxRepository;

public class BoxRepository : IBoxRepository
{
    private readonly Container _container;

    public BoxRepository(BoxRepositoryConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var cosmosClient = new CosmosClient(config.ConnectionString);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }

    public async Task<Box> Add(Box box)
    {
        var cosmosAware = CosmosAwareBox.ToCosmosAware(box);
        var response = await _container.CreateItemAsync(cosmosAware, cosmosAware.GetPartitionKey());
        return response.Resource;
    }

    public async Task Delete(CollectionId collectionId, BoxId boxId)
    {
        await _container.DeleteItemAsync<CosmosAwareBox>(boxId.ToString(), new PartitionKey(collectionId.ToString()));
    }

    public async Task<Box> Get(CollectionId collectionId, BoxId boxId)
    {
        try
        {
            var cosmosAware =
                await _container.ReadItemAsync<CosmosAwareBox>(boxId.ToString(),
                    new PartitionKey(collectionId.ToString()));
            return cosmosAware.Resource;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            throw new BoxNotFoundException(collectionId, boxId);
        }
    }

    public async Task<Box> PersistUpdate(Box updatedBox)
    {
        if (updatedBox is not CosmosAwareBox cosmosAwareBox)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        try
        {
            var cosmosResponse = await _container.ReplaceItemAsync(cosmosAwareBox, cosmosAwareBox.Id,
                    cosmosAwareBox.GetPartitionKey(), new ItemRequestOptions()
                    {
                        IfMatchEtag = cosmosAwareBox.ETag
                    });
            return cosmosResponse.Resource;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            throw new BoxNotFoundException(updatedBox.CollectionId, updatedBox.BoxId);
        }
    }

    public async Task<List<Box>> GetCollection(CollectionId collectionId)
    {
        var queryAble = _container
            .GetItemLinqQueryable<CosmosAwareBox>(requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(collectionId.ToString())
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