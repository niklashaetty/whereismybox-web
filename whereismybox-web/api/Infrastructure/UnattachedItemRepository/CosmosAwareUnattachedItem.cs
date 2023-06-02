using Domain.Models;
using Domain.Primitives;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Infrastructure.UnattachedItemRepository;

public class CosmosAwareUnattachedItem : UnattachedItem, ICosmosAware
{
    [JsonProperty(PropertyName = "_etag")] public string ETag { get; set; }

    [JsonProperty(PropertyName = "id")] public string Id { get; set; }


    public CosmosAwareUnattachedItem(CollectionId collectionId, ItemId itemId, string name, string description,
        BoxId? previousBoxId) : base(collectionId,
        itemId, name, description, previousBoxId)
    {
        Id = itemId.ToString();
    }

    public static CosmosAwareUnattachedItem ToCosmosAware(UnattachedItem unattachedItem)
    {
        ArgumentNullException.ThrowIfNull(unattachedItem);
        return new CosmosAwareUnattachedItem(unattachedItem.CollectionId, unattachedItem.ItemId, unattachedItem.Name,
            unattachedItem.Description, unattachedItem.PreviousBoxId);
    }

    public PartitionKey PartitionKey()
    {
        return new PartitionKey(CollectionId.ToString());
    }
}