using System.Net;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.UnattachedItemRepository;

public class UnattachedItemRepository : IUnattachedItemRepository
{
    private readonly Container _container;

    public UnattachedItemRepository(UnattachedItemRepositoryRepositoryConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var cosmosClient = new CosmosClient(config.ConnectionString);
        _container = cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }
    
    public async Task<UnattachedItemCollection> Create(UnattachedItemCollection unattachedItemCollection)
    {
        var cosmosAware = CosmosAwareUnattachedItemCollection.ToCosmosAware(unattachedItemCollection);
        var response = await _container.CreateItemAsync(cosmosAware, cosmosAware.GetPartitionKey());
        return response.Resource;
    }

    public async Task<UnattachedItemCollection> Get(Guid userId)
    {
        try
        {
            var cosmosAware =
                await _container.ReadItemAsync<CosmosAwareUnattachedItemCollection>(userId.ToString(),
                    new PartitionKey(userId.ToString()));
            return cosmosAware.Resource;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            throw new UnattachedItemsNotFoundException(userId, e);
        }
    }

    public async Task<UnattachedItemCollection> PersistUpdate(UnattachedItemCollection updatedUnattachedItemCollection)
    {
        if (updatedUnattachedItemCollection is not CosmosAwareUnattachedItemCollection cosmosAware)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var cosmosResponse =
            await _container.ReplaceItemAsync(cosmosAware, cosmosAware.Id,
                cosmosAware.GetPartitionKey(), new ItemRequestOptions()
                {
                    IfMatchEtag = cosmosAware.ETag
                });
        return cosmosResponse.Resource;
    }
}