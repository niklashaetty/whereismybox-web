using System.Net;
using Domain.Exceptions;
using Domain.Models;
using Domain.Primitives;
using Domain.Repositories;
using Infrastructure.BoxRepository;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace NarrowIntegrationTests.Fakes;

public class FakeBoxRepository : IBoxRepository
{
    private readonly List<TestableBox> _database = new();

    public Task<Box> Add(Box box)
    {
        var cosmosAwareBox = CosmosAwareBox.ToCosmosAware(box);
        cosmosAwareBox.ETag = Guid.NewGuid().ToString();
        var testableBox = new TestableBox(cosmosAwareBox);

        _database.Add(testableBox);
        return Task.FromResult<Box>(testableBox.AsCosmosAware());
    }

    public Task Delete(CollectionId collectionId, BoxId boxId)
    {
        _database.RemoveAll(b => b.CollectionId.Equals(collectionId) && b.BoxId == boxId);
        return Task.CompletedTask;
    }

    public Task<Box> Get(CollectionId collectionId, BoxId boxId)
    {
        var box = _database.SingleOrDefault(b => b.CollectionId.Equals(collectionId) && b.BoxId == boxId);
        if (box is null)
        {
            throw new BoxNotFoundException(collectionId, boxId);
        }

        return Task.FromResult<Box>(box.AsCosmosAware());
    }

    public Task<Box> PersistUpdate(Box updatedBox)
    {
        if (updatedBox is not CosmosAwareBox newCosmosAwareBox)
        {
            throw new InvalidOperationException("Not a cosmos aware item");
        }

        var box = _database.SingleOrDefault(b =>
            b.CollectionId.Equals(updatedBox.CollectionId) && b.BoxId == updatedBox.BoxId);
        if (box is null)
        {
            throw new BoxNotFoundException(updatedBox.CollectionId, updatedBox.BoxId);
        }

        var oldCosmosAwareBox = box.AsCosmosAware();
        if (newCosmosAwareBox.ETag != oldCosmosAwareBox.ETag)
        {
            throw new CosmosException("Opportunistic locking failed!", HttpStatusCode.Conflict, 409, "1", 1.0);
        }

        _database.RemoveAll(b => b.CollectionId.Equals(updatedBox.CollectionId) && b.BoxId == updatedBox.BoxId);
        newCosmosAwareBox.ETag = Guid.NewGuid().ToString();
        _database.Add(new TestableBox(newCosmosAwareBox));
        return Task.FromResult<Box>(newCosmosAwareBox);
    }

    public Task<List<Box>> GetCollection(CollectionId collectionId)
    {
        var testableBoxes = _database.Where(b => b.CollectionId.Equals(collectionId)).ToList();
        var result = new List<Box>();
        foreach (var testableBox in testableBoxes)
        {
            result.Add(testableBox.AsCosmosAware());
        }

        return Task.FromResult(result);
    }

    public Task ScheduleForDeletion(CollectionId collectionId, Box box, int daysUntilDeletion)
    {
        throw new NotImplementedException();
    }
}

public class TestableBox
{
    public CollectionId CollectionId { get; set; }
    public BoxId BoxId { get; set; }
    public string SerializedCosmosAwareBox { get; set; }

    public TestableBox(CosmosAwareBox cosmosAwareBox)
    {
        ArgumentNullException.ThrowIfNull(cosmosAwareBox);
        CollectionId = cosmosAwareBox.CollectionId;
        BoxId = cosmosAwareBox.BoxId;
        SerializedCosmosAwareBox = JsonConvert.SerializeObject(cosmosAwareBox);
    }

    public CosmosAwareBox AsCosmosAware()
    {
        return JsonConvert.DeserializeObject<CosmosAwareBox>(SerializedCosmosAwareBox) ??
               throw new InvalidOperationException("Test error");
    }
}