using Domain.Models;
using Domain.Primitives;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Infrastructure.CollectionRepository;

public class CosmosAwareCollection : Collection
{
    [JsonProperty(PropertyName = "_etag")] 
    public string ETag { get; set; } = "*";
    
    [JsonProperty(PropertyName = "id")] 
    public string Id { get; set; }
    
    protected CosmosAwareCollection(CollectionId collectionId, UserId ownerId, List<Box> boxes,
        List<UnattachedItem> unattachedItems) : base(collectionId, ownerId, boxes, unattachedItems)
    {
        Id = collectionId.Value;
    }

    public static CosmosAwareCollection ToCosmosAwareCollection(Collection collection)
    {
        ArgumentNullException.ThrowIfNull(collection);
        return new CosmosAwareCollection(collection.CollectionId, collection.OwnerId,
            collection.Boxes, collection.UnattachedItems);
    }

    public PartitionKey PartitionKey() => new (CollectionId.ToString());
}