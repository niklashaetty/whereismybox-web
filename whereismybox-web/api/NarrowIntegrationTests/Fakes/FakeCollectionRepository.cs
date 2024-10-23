using System.Net;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.BoxRepository;
using Infrastructure.CollectionRepository;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace NarrowIntegrationTests.Fakes;

public class FakeCollectionRepository : ICollectionRepository
{
    private readonly List<TestableCollection> _database = new();

    public Task<Collection> Create(Collection collection)
    {
        var cosmosAwareCollection = CosmosAwareCollection.ToCosmosAware(collection);
        cosmosAwareCollection.ETag = Guid.NewGuid().ToString();
        var testableCollection = new TestableCollection(cosmosAwareCollection);

        _database.Add(testableCollection);
        return Task.FromResult<Collection>(testableCollection.AsCosmosAware());
    }

    public Task Delete(CollectionId collectionId)
    {
        _database.RemoveAll(b => b.CollectionId.Equals(collectionId));
        return Task.CompletedTask;
    }

    public Task<Collection> Get(CollectionId collectionId)
    {
        var collection = _database.SingleOrDefault(b => b.CollectionId.Equals(collectionId));
        if (collection is null)
        {
            throw new CollectionNotFoundException(collectionId);
        }

        return Task.FromResult<Collection>(collection.AsCosmosAware());
    }

    public Task<Collection> PersistUpdate(Collection updatedCollection)
    {
        if (updatedCollection is not CosmosAwareCollection newCosmosAwareCollection)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var box = _database.SingleOrDefault(b =>
            b.CollectionId.Equals(updatedCollection.CollectionId));
        if (box is null)
        {
            throw new CollectionNotFoundException(updatedCollection.CollectionId);
        }

        var oldCosmosAwareBox = box.AsCosmosAware();
        if (newCosmosAwareCollection.ETag != oldCosmosAwareBox.ETag)
        {
            throw new CosmosException("Opportunistic locking failed!", HttpStatusCode.Conflict, 409, "1", 1.0);
        }

        _database.RemoveAll(b => b.CollectionId.Equals(updatedCollection.CollectionId));
        newCosmosAwareCollection.ETag = Guid.NewGuid().ToString();
        _database.Add(new TestableCollection(newCosmosAwareCollection));
        return Task.FromResult<Collection>(newCosmosAwareCollection);
    }

    public Task<List<Collection>> GetOwnedCollections(UserId userId)
    {
        var testableCollections = _database.Where(b => b.Owner.Equals(userId)).ToList();
        var result = new List<Collection>();
        foreach (var testableCollection in testableCollections)
        {
            result.Add(testableCollection.AsCosmosAware());
        }

        return Task.FromResult(result);
    }

    public Task<List<Collection>> GetCollectionsWhereUserIsContributor(UserId userId)
    {
        var testableCollections = _database.Where(collection => collection.Contributors.Contains(userId)).ToList();
        var result = new List<Collection>();
        foreach (var testableCollection in testableCollections)
        {
            result.Add(testableCollection.AsCosmosAware());
        }

        return Task.FromResult(result);
    }

    public class TestableCollection
    {
        public CollectionId CollectionId { get; set; }
        public UserId Owner { get; set; }
        public List<UserId> Contributors { get; set; }
        public string SerializedCosmosAwareCollection { get; set; }

        public TestableCollection(CosmosAwareCollection cosmosAwareCollection)
        {
            ArgumentNullException.ThrowIfNull(cosmosAwareCollection);
            CollectionId = cosmosAwareCollection.CollectionId;
            Owner = cosmosAwareCollection.Owner;
            Contributors = cosmosAwareCollection.Contributors;
            SerializedCosmosAwareCollection = JsonConvert.SerializeObject(cosmosAwareCollection);
        }

        public CosmosAwareCollection AsCosmosAware()
        {
            return JsonConvert.DeserializeObject<CosmosAwareCollection>(SerializedCosmosAwareCollection) ??
                   throw new InvalidOperationException("Test error");
        }
    }
}