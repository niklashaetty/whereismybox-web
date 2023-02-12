using System.Dynamic;
using Newtonsoft.Json;

namespace Domain.Models;

public class UnattachedItemCollection
{
    [JsonProperty] public Guid UserId { get; private set; }
    [JsonProperty] public List<UnattachedItem> UnattachedItems { get; private set; }

    public UnattachedItemCollection(Guid userId, List<UnattachedItem> unattachedItems)
    {
        ArgumentNullException.ThrowIfNull(unattachedItems);
        UserId = userId;
        UnattachedItems = unattachedItems;
    }
    
    public static UnattachedItemCollection Create(Guid userId)
    {
        return new UnattachedItemCollection(userId, new List<UnattachedItem>());
    }
    
    [JsonConstructor]
    protected UnattachedItemCollection()
    {
    }

    public int RemoveIfExists(Guid itemId)
    {
        return UnattachedItems.RemoveAll(i => i.ItemId == itemId);
    }
    
    public bool TryGetItem(Guid itemId, out Item item)
    {
        item = UnattachedItems.FirstOrDefault(i => i.ItemId == itemId);
        return item is not null;
    }
    
    public void Add(Item item, Guid? previousBoxId=null)
    {
        var unattachedItem = new UnattachedItem(item, previousBoxId);
        UnattachedItems.Add(unattachedItem);
    }
}