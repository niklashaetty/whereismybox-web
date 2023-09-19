using Domain.Models;
using Domain.Primitives;
using Infrastructure.BoxRepository;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Infrastructure.CollectionRepository;

public class CosmosAwareCollection : Collection, ICosmosAware
{
    [JsonProperty(PropertyName = "_etag")] public string ETag { get; set; }

    [JsonProperty(PropertyName = "id")] public string Id { get; set; }

    public CosmosAwareCollection(CollectionId collectionId, string name, UserId owner, List<UserId> contributors) :
        base(collectionId, name, owner, contributors)
    {
        Id = CollectionId.Value;
    }

    public static CosmosAwareCollection ToCosmosAware(Collection collection)
    {
        return new CosmosAwareCollection(collection.CollectionId, collection.Name, collection.Owner,
            collection.Contributors);
    }

    public PartitionKey GetPartitionKey()
    {
        return new PartitionKey(Owner.ToString());
    }
}