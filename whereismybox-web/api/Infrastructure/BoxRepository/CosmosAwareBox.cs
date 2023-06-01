using Domain.Models;
using Domain.Primitives;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Infrastructure.BoxRepository;

public class CosmosAwareBox : Box, ICosmosAware
{
    [JsonProperty(PropertyName = "_etag")] public string ETag { get; set; }

    [JsonProperty(PropertyName = "id")] public string Id { get; set; }

    public CosmosAwareBox(CollectionId collectionId, BoxId boxId, int number, string name, List<Item> items) : base(
        collectionId, boxId, number, name, items)
    {
        Id = boxId.ToString();
    }

    public static CosmosAwareBox ToCosmosAware(Box box)
    {
        return new CosmosAwareBox(box.CollectionId, box.BoxId, box.Number, box.Name, box.Items);
    }

    public PartitionKey GetPartitionKey()
    {
        return new PartitionKey(CollectionId.ToString());
    }
}