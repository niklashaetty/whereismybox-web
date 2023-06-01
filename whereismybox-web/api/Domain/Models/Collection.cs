using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class Collection
{
    [JsonProperty] public CollectionId CollectionId { get; }
    [JsonProperty] public UserId OwnerId { get; }
    [JsonProperty] public List<Box> Boxes { get; }
    [JsonProperty] public List<UnattachedItem> UnattachedItems { get; }

    [JsonConstructor]
    protected Collection()
    {
    }
    
    protected Collection(CollectionId collectionId, UserId ownerId, List<Box> boxes,
        List<UnattachedItem> unattachedItems)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(ownerId);
        ArgumentNullException.ThrowIfNull(boxes);
        ArgumentNullException.ThrowIfNull(unattachedItems);
        CollectionId = collectionId;
        OwnerId = ownerId;
        Boxes = boxes;
        UnattachedItems = unattachedItems;
    }

    public static Collection NewCollection(CollectionId collectionId, UserId ownerId)
    {
        return new Collection(collectionId, ownerId,
            new List<Box>(),
            new List<UnattachedItem>());
    }
}