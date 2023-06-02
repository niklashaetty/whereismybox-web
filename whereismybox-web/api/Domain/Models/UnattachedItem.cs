using Domain.Primitives;
using Newtonsoft.Json;

namespace Domain.Models;

public class UnattachedItem : Item
{
    [JsonProperty] public CollectionId CollectionId { get; private set; }
    [JsonProperty] public BoxId? PreviousBoxId { get; private set; }
    [JsonProperty] public int? PreviousBoxNumber { get; private set; }


    [JsonConstructor]
    protected UnattachedItem()
    {
    }

    public static UnattachedItem ToUnattached(Item item, CollectionId collectionId, BoxId? previousBoxId)
    {
        return new UnattachedItem(collectionId, item.ItemId, item.Name, item.Description, previousBoxId);
    }
    
    public UnattachedItem(CollectionId collectionId, ItemId itemId, string name, string description, BoxId? previousBoxId) :
        base(itemId, name, description)
    {
        ArgumentNullException.ThrowIfNull(collectionId);
        ArgumentNullException.ThrowIfNull(itemId);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        CollectionId = collectionId;
        PreviousBoxId = previousBoxId;
    }

    public void AddPreviousBoxNumber(int previousBoxNumber)
    {
        PreviousBoxNumber = previousBoxNumber;
    }

    public void RemovePreviousBox()
    {
        PreviousBoxNumber = null;
        PreviousBoxNumber = null;
    }
}