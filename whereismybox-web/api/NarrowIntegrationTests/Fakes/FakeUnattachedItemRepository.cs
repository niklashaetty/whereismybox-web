using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.UnattachedItemRepository;
using Newtonsoft.Json;

namespace NarrowIntegrationTests.Fakes;

public class FakeUnattachedItemRepository : IUnattachedItemRepository
{
    private readonly List<TestableUnattachedItem> _database = new();

    public Task<UnattachedItem> Create(UnattachedItem unattachedItem)
    {
        var cosmosAwareUnattachedItem = CosmosAwareUnattachedItem.ToCosmosAware(unattachedItem);
        cosmosAwareUnattachedItem.ETag = Guid.NewGuid().ToString();
        var testableUnattachedItem = new TestableUnattachedItem(cosmosAwareUnattachedItem);

        _database.Add(testableUnattachedItem);
        return Task.FromResult<UnattachedItem>(testableUnattachedItem.AsCosmosAware());
    }

    public Task<List<UnattachedItem>> GetCollection(CollectionId collectionId)
    {
        var testableUnattachedItems = 
            _database.Where(b => b.CollectionId.Equals(collectionId)).ToList();
        var result = new List<UnattachedItem>();
        foreach (var testableBoxUnattachedItem in testableUnattachedItems)
        {
            result.Add(testableBoxUnattachedItem.AsCosmosAware());
        }

        return Task.FromResult(result);
    }

    public Task Delete(CollectionId collectionId, ItemId itemId)
    {
        _database.RemoveAll(b => b.CollectionId.Equals(collectionId) && b.ItemId == itemId);
        return Task.CompletedTask;
    }
}

public class TestableUnattachedItem
{
    public CollectionId CollectionId { get; set; }
    public ItemId ItemId { get; set; }
    public string SerializedCosmosAwareUnattachedItem { get; set; }

    public TestableUnattachedItem(CosmosAwareUnattachedItem cosmosAwareUnattachedItem)
    {
        ArgumentNullException.ThrowIfNull(cosmosAwareUnattachedItem);
        CollectionId = cosmosAwareUnattachedItem.CollectionId;
        ItemId = cosmosAwareUnattachedItem.ItemId;
        SerializedCosmosAwareUnattachedItem = JsonConvert.SerializeObject(cosmosAwareUnattachedItem);
    }

    public CosmosAwareUnattachedItem AsCosmosAware()
    {
        return JsonConvert.DeserializeObject<CosmosAwareUnattachedItem>(SerializedCosmosAwareUnattachedItem) ??
               throw new InvalidOperationException("Test error");
    }
}